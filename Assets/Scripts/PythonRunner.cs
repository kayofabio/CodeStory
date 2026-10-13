
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class PythonRunner
{
    private const int TempoLimiteMs = 10000;

    public async Task<(string saida, string erro)> ExecutarAsync(string codigo)
    {
        string caminhoScript = Path.Combine(
            Application.temporaryCachePath,
            "script_teste.py"
        );

        try
        {
            File.WriteAllText(caminhoScript, codigo);



            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "\"" + caminhoScript + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
                CreateNoWindow = true
            };

            info.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";



            using Process processo = new Process();
            processo.StartInfo = info;
            processo.Start();

            Task<string> tarefaSaida =
                processo.StandardOutput.ReadToEndAsync();

            Task<string> tarefaErro =
                processo.StandardError.ReadToEndAsync();

            Task tarefaEspera = Task.Run(() => processo.WaitForExit());

            Task resultado = await Task.WhenAny(
                tarefaEspera,
                Task.Delay(TempoLimiteMs)
            );

            if (resultado != tarefaEspera)
            {
                processo.Kill();
                await tarefaEspera;

                return ("", "Tempo limite de execução excedido.");
            }

            await Task.WhenAll(tarefaSaida, tarefaErro);

            string saida = tarefaSaida.Result.TrimEnd();
            string erro = tarefaErro.Result.TrimEnd();

            if (processo.ExitCode != 0)
            {
                return (saida, erro);
            }

            return (saida, "");
        }
        catch (Exception ex)
        {
            return (
                "",
                "Não foi possível executar o Python: " + ex.Message
            );
        }
    }
}
