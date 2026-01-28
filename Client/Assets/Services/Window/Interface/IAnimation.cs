using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAnimation
{
    UniTask DOPlayAsync(CancellationToken token = default);
}
