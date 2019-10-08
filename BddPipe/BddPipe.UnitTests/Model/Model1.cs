namespace BddPipe.UnitTests.Model
{
    public sealed class Model1
    {
        public string Name { get; }

        public Model1() : this(nameof(Model1)) { }
        public Model1(string name)
        {
            Name = name;
        }
    }
}
