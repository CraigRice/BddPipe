namespace BddPipe.UnitTests.Model
{
    internal sealed class Model1(string name)
    {
        public string Name { get; } = name;

        public Model1() : this(nameof(Model1)) { }
    }
}
