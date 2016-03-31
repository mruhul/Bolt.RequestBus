namespace Sample.CreateBook
{
    public class CreateBookRequest : Bolt.RequestBus.IRequest
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }
    }
}
