using HomeCare.Repositorio.Contexto;

namespace HomeCare.Repositorio;

public abstract class BaseRepositorio
{
    protected readonly HomeCareContexto _contexto;

    protected BaseRepositorio(HomeCareContexto contexto)
    {
        _contexto = contexto;
    }
}
