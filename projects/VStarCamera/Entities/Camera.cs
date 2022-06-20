using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace VStarCameraZone.Entities;

/// <summary>
/// VStar camera entity.
/// </summary>
public class Camera
{
    private const int IrParam = 14;

    /// <summary>
    /// Camera identifier.
    /// </summary>
    public string Id { get; private set; }

    public CameraCredentials Credentials { get; set; } = CameraCredentials.Empty;

    public string Ip { get; set; }

    public string IpMask { get; set; }

    public string IpGateway { get; set; }

    public string PrimaryDns { get; set; }

    public string SecondaryDns { get; set; }

    public ushort Port { get; set; }

    public string Name { get; set; }

    public string FirmwareVersion { get; set; }

    public Camera(string id)
    {
        this.Id = id;
    }

    private static readonly Regex ParametersGetResponseRegex =
        new(@"^var (.+)=(.+);", RegexOptions.Compiled | RegexOptions.Singleline);

    public async Task<CameraParameters> GetParameters(CancellationToken cancellationToken)
    {
        var request = CreateParametersGetRequest();
        var client = GetClient();
        var response = await client.ExecuteAsync(request, cancellationToken);
        ProcessParametersResponse(response);

        var param = new CameraParameters();

        var paramSetMap = new Dictionary<string, Action<CameraParameters, string>>
        {
            ["ircut"] = (cp, val) => cp.Ir = val == "1",
            ["MainStreamWidth"] = (cp, val) => cp.MainStreamWidth = int.Parse(val),
            ["MainStreamHeight"] = (cp, val) => cp.MainStreamHeight = int.Parse(val),
            ["enc_bitrate"] = (cp, val) => cp.Bitrate = int.Parse(val),
            ["enc_framerate"] = (cp, val) => cp.Framerate = int.Parse(val),
        };

        foreach (var line in response.Content.Split(new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var match = ParametersGetResponseRegex.Match(line);
            if (match.Success)
            {
                var variable = match.Groups[1].Value.Trim();
                if (paramSetMap.TryGetValue(variable, out Action<CameraParameters, string> action))
                {
                    var value = match.Groups[2].Value.Trim();
                    action(param, value);
                }
            }
        }

        return param;
    }

    public async Task SetIr(bool value, CancellationToken cancellationToken)
    {
        var request = CreateParametersSetRequest(IrParam, value ? "1" : "0");
        var client = GetClient();
        var response = await client.ExecuteAsync(request, cancellationToken);
        ProcessParametersResponse(response);
    }

    /// <summary>
    /// Dump information about camera into string.
    /// </summary>
    /// <returns>Camera info string.</returns>
    public string DumpInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id: " + Id);
        sb.AppendLine("Name: " + Name);
        sb.AppendLine($"IP with port: {Ip}:{Port}");
        sb.AppendLine("IP mask: " + IpMask);
        sb.AppendLine("IP gateway: " + IpGateway);
        sb.AppendLine("Primary DNS: " + PrimaryDns);
        sb.AppendLine("Secondary DNS: " + SecondaryDns);
        sb.AppendLine("Firmware version: " + FirmwareVersion);
        return sb.ToString();
    }

    private RestClient GetClient() => new RestClient($"http://{Ip}:{Port}/");

    private RestRequest CreateParametersSetRequest(int param, string value)
    {
        // What is interesting, I found that password verification was missed
        // for this endpoint.
        var request = CreateGeneralRequest("camera_control.cgi");
        request.AddQueryParameter("param", param.ToString());
        request.AddQueryParameter("value", value);
        return request;
    }

    private RestRequest CreateParametersGetRequest() => CreateGeneralRequest("get_camera_params.cgi");

    private RestRequest CreateGeneralRequest(string resource)
    {
        var request = new RestRequest(resource);
        request.AddQueryParameter("loginuse", Credentials.Login);
        request.AddQueryParameter("loginpas", Credentials.Password);
        request.AddQueryParameter("_", DateTime.Now.Ticks.ToString());
        return request;
    }

    private static readonly Regex ParametersResultResponseRegex =
        new(@"result=\""(.+)\"";", RegexOptions.Compiled | RegexOptions.Multiline);

    private void ProcessParametersResponse(RestResponse response)
    {
        if (!response.IsSuccessful)
        {
            throw new CameraException("Invalid request: " + response.ErrorMessage);
        }
        var match = ParametersResultResponseRegex.Match(response.Content);
        if (match.Success)
        {
            var ret = match.Groups[1].Value;
            if (!ret.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                throw new CameraException(ret);
            }
        }
    }
}
