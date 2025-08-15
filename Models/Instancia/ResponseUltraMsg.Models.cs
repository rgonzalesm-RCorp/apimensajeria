public class ResponseQrCode
{
    public string qrCode { get; set; }
    public int usuarioId { get; set; }
    public int loginUserId { get; set; }
}
public class ResponseRegistroUsuario
{
    public string status { get; set; }
    public string message { get; set; }
    public ResponseQrCode data { get; set; }
}

public class ApiResponseStatusQr
{
    public StatusQr status { get; set; }
}
public class ApiResponseStatusError
{
    public string error { get; set; }
}
public class StatusQr
{
    public AccountStatusQR accountStatus { get; set; }
}

public class AccountStatusQR
{
    public string status { get; set; }
    public string substatus { get; set; }
}
