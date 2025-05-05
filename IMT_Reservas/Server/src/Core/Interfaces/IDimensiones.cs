public interface IDimensiones
{
    double? Alto  { get; }
    double? Ancho { get; }
    double? Largo { get; }
    void SetDimensiones(double? alto, double? ancho, double? largo);
    (double? alto, double? ancho, double? largo) GetDimensiones();
}
