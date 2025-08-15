public class EmpresaSave
{
    public string Nombre { get; set; }
    public string Srv { get; set; }
    public string DB { get; set; }
    public string Usuario { get; set; }
    public string Pasword { get; set; }
    public string CodigoCC { get; set; }
    public int CodigoUsuario { get; set; }
}
public class EmpresaConfiguracionSave
{
    public int EmpresaId { get; set; }
    public string Clave { get; set; }
    public string Valor { get; set; }
}
public class UsuarioSave
{
    public int EmpresaId { get; set; }
    public int InstanciaId { get; set; }
    public string NroTelefono { get; set; }
    public int CodigoUsuario { get; set; }
}

public class SaveEmpresaRequest
{
    public int EmpresaId { get; set; }
    public EmpresaSave Empresa { get; set; }
    public List<EmpresaConfiguracionSave> Configuraciones { get; set; }
    public UsuarioSave Usuario { get; set; }
}