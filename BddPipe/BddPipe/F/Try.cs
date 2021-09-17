using System.Threading.Tasks;

namespace BddPipe
{
    internal delegate Result<A> Try<A>();

    internal delegate Task<Result<A>> TryAsync<A>();
}
