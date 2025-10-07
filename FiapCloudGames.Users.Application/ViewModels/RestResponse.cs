namespace FiapCloudGames.Users.Application.ViewModels;

public class RestResponse
{
    public Dictionary<string, string[]> Errors { get; set; }

    public static RestResponse Success() => new();

    public static RestResponse Error(Dictionary<string, string[]> errors) => new() { Errors = errors };
}

public class RestResponse<T> : RestResponse
{
    public T? Data { get; private set; }

    public static RestResponse<T> Success(T? data) => new() { Data = data };

    public static new RestResponse<T> Error(Dictionary<string, string[]> errors) => new() { Errors = errors };
}
