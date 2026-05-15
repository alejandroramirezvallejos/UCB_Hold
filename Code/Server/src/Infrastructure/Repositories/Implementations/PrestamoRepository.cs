using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class PrestamoRepository : Repository<PrestamoEntity, PrestamoDto>
{
    private readonly PrestamoMapper _mapper;

    public PrestamoRepository(ApplicationDbContext dbContext, PrestamoMapper mapper)
        : base(dbContext)
    {
        _mapper = mapper;
    }

    protected override PrestamoDto MapToDto(PrestamoEntity entity) => _mapper.ToDto(entity);

    public override async Task<Result<List<PrestamoDto>>> GetAll(QueryFilter? filter = null)
    {
        var rows = await ProjectFlat(DbContext.Prestamos.AsNoTracking()).ToListAsync();
        
        return Result<List<PrestamoDto>>.Success(rows.Select(MapRowToDto).ToList());
    }

    public async Task<Result<List<PrestamoDto>>> GetHistorialWithDetalles(string carnetUsuario, EstadoPrestamo? estado)
    {
        var query = DbContext.Prestamos.AsNoTracking().Where(prestamo => prestamo.Carnet == carnetUsuario);
        
        if (estado.HasValue) 
            query = query.Where(prestamo => prestamo.EstadoPrestamo == estado.Value);

        var rows = await ProjectFlat(query).ToListAsync();
        
        return Result<List<PrestamoDto>>.Success(rows.Select(MapRowToDto).ToList());
    }

    public override async Task<Result<PrestamoDto>> Get(int id)
    {
        var rows = await ProjectFlat(DbContext.Prestamos.AsNoTracking().Where(prestamo => prestamo.Id == id)).ToListAsync();
        var first = rows.FirstOrDefault();
        
        return first == null ? Result<PrestamoDto>.NotFound() : Result<PrestamoDto>.Success(MapRowToDto(first));
    }

    private IQueryable<FlatRow> ProjectFlat(IQueryable<PrestamoEntity> source)
        => from prestamo in source
           join usuario in DbContext.Usuarios.AsNoTracking()
               on prestamo.Carnet equals usuario.Carnet into usuarioJoin
           from usuario in usuarioJoin.DefaultIfEmpty()
           join detalle in DbContext.DetallesPrestamos.AsNoTracking().Where(detalle => !detalle.EstadoEliminado)
               on prestamo.Id equals detalle.IdPrestamo into detalleJoin
           from detalle in detalleJoin.DefaultIfEmpty()
           join equipo in DbContext.Equipos.AsNoTracking()
               on detalle.IdEquipo equals equipo.Id into equipoJoin
           from equipo in equipoJoin.DefaultIfEmpty()
           join grupo in DbContext.GruposEquipos.AsNoTracking()
               on equipo.IdGrupoEquipo equals grupo.Id into grupoJoin
           from grupo in grupoJoin.DefaultIfEmpty()
           join gavetero in DbContext.Gaveteros.AsNoTracking()
               on equipo.IdGavetero equals gavetero.Id into gaveteroJoin
           from gavetero in gaveteroJoin.DefaultIfEmpty()
           join mueble in DbContext.Muebles.AsNoTracking()
               on gavetero.IdMueble equals mueble.Id into muebleJoin
           from mueble in muebleJoin.DefaultIfEmpty()
           select new FlatRow
           {
               PrestamoId = prestamo.Id,
               Carnet = prestamo.Carnet,
               UsuarioNombre = usuario != null ? usuario.Nombre : null,
               UsuarioApellido = usuario != null ? usuario.ApellidoPaterno : null,
               UsuarioTelefono = usuario != null ? usuario.Telefono : null,
               EstadoPrestamo = prestamo.EstadoPrestamo,
               FechaSolicitud = prestamo.FechaSolicitud,
               FechaPrestamoEsperada = prestamo.FechaPrestamoEsperada,
               FechaPrestamo = prestamo.FechaPrestamo,
               FechaDevolucionEsperada = prestamo.FechaDevolucionEsperada,
               FechaDevolucion = prestamo.FechaDevolucion,
               Observacion = prestamo.Observacion,
               IdContrato = prestamo.IdContrato,
               NombreGrupoEquipo = grupo != null ? grupo.Nombre : null,
               CodigoImt = equipo != null ? (int?)equipo.CodigoImt : null,
               UbicacionEquipo = equipo != null ? equipo.Ubicacion : null,
               NombreGavetero = gavetero != null ? gavetero.Nombre : null,
               NombreMueble = mueble != null ? mueble.Nombre : null,
               UbicacionMueble = mueble != null ? mueble.Ubicacion : null
           };

    private static PrestamoDto MapRowToDto(FlatRow row) => new()
    {
        Id = row.PrestamoId,
        CarnetUsuario = row.Carnet,
        NombreUsuario = row.UsuarioNombre,
        ApellidoPaternoUsuario = row.UsuarioApellido,
        TelefonoUsuario = row.UsuarioTelefono,
        EstadoPrestamo = EstadoPrestamoState.ToText(row.EstadoPrestamo),
        FechaSolicitud = row.FechaSolicitud,
        FechaPrestamoEsperada = row.FechaPrestamoEsperada,
        FechaPrestamo = row.FechaPrestamo,
        FechaDevolucionEsperada = row.FechaDevolucionEsperada,
        FechaDevolucion = row.FechaDevolucion,
        Observacion = row.Observacion,
        IdContrato = row.IdContrato,
        NombreGrupoEquipo = row.NombreGrupoEquipo,
        CodigoImt = row.CodigoImt?.ToString(),
        UbicacionEquipo = row.UbicacionEquipo,
        NombreGavetero = row.NombreGavetero,
        NombreMueble = row.NombreMueble,
        UbicacionMueble = row.UbicacionMueble
    };

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.Id == id && !prestamo.EstadoEliminado);
        
        if (entity == null) 
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();
        return Result<object>.Success(null!);
    }

    public PrestamoDto ConvertToDto(PrestamoEntity entity) => MapToDto(entity);

    private class FlatRow
    {
        public int PrestamoId { get; set; }
        public string? Carnet { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? UsuarioApellido { get; set; }
        public string? UsuarioTelefono { get; set; }
        public EstadoPrestamo EstadoPrestamo { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaPrestamoEsperada { get; set; }
        public DateTime? FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string? Observacion { get; set; }
        public int? IdContrato { get; set; }
        public string? NombreGrupoEquipo { get; set; }
        public int? CodigoImt { get; set; }
        public string? UbicacionEquipo { get; set; }
        public string? NombreGavetero { get; set; }
        public string? NombreMueble { get; set; }
        public string? UbicacionMueble { get; set; }
    }
}
