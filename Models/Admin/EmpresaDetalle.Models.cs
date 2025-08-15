public class EmpresaDetalle
{
    
    public int EmpresaId_ { get; set; }
    public int EmpresaId { get; set; }
    public string Nombre { get; set; }
    public string CodigoCC { get; set; }
    public DateTime FechaRegistro { get; set; }

    public int? LimiteDiario { get; set; }
    public int? LimiteMensual { get; set; }
    public string EnviarDespuesDelLimite { get; set; }
    public string CobrarDespuesDelLimite { get; set; }

    public int InstanciaId { get; set; }

    public string InstanceIdUltraMsg { get; set; }
    public string Token { get; set; }
    public string Descripcion { get; set; }
    public bool MultiEmpresa { get; set; }
    public string NumeroTelefono { get; set; }
}
