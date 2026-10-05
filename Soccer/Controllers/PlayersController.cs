using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Models;

namespace Soccer.Controllers;

// Використання Primary Constructor (C# 12)
public class PlayersController(SoccerContext context) : Controller
{
    private readonly SoccerContext _context = context;

    // GET: Players
    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 10; // кількість елементів на сторінці

        // AsNoTracking() суттєво зменшує навантаження на пам'ять для read-only запитів
        var query = _context.Players.Include(x => x.Team).AsNoTracking();

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pageViewModel = new PageViewModel(count, page, pageSize);
        var viewModel = new IndexViewModel(items, pageViewModel);

        return View(viewModel);
    }

    // GET: Players/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var player = await _context.Players
            .Include(p => p.Team)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return player is null ? NotFound() : View(player);
    }

    // GET: Players/Create
    public IActionResult Create()
    {
        ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
        return View();
    }

    // POST: Players/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,BirthYear,Position,TeamId")] Player player)
    {
        if (DateTime.Now.Year - player.BirthYear <= 0)
        {
            ModelState.AddModelError("Age", "Вік гравця повинен бути більшим за нуль");
        }

        if (ModelState.IsValid)
        {
            _context.Add(player);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
        return View(player);
    }

    // GET: Players/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var player = await _context.Players.FindAsync(id);
        if (player is null) return NotFound();

        ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
        return View(player);
    }

    // POST: Players/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BirthYear,Position,TeamId")] Player player)
    {
        if (id != player.Id) return NotFound();

        if (DateTime.Now.Year - player.BirthYear <= 0)
        {
            ModelState.AddModelError("Age", "Вік гравця повинен бути більшим за нуль");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(player);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(player.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
        return View(player);
    }

    // GET: Players/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var player = await _context.Players
            .Include(p => p.Team)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return player is null ? NotFound() : View(player);
    }

    // POST: Players/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PlayerExists(int id) => _context.Players.Any(e => e.Id == id);
}