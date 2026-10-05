namespace Soccer.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Player> Players { get; }
        public PageViewModel PageViewModel { get; }
        public IndexViewModel(IEnumerable<Player> players, PageViewModel viewModel)
        {
            Players = players;
            PageViewModel = viewModel;
        }
    }
}
