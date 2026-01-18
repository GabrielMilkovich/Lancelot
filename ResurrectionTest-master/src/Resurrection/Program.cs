using Core;
using Resurrection.App.Controller;
using Resurrection.Configurations;
using Resurrection.Core.Core.Domain.Entities;
using Resurrection.Domain.Constantes;

class Program
{
    public async static Task Main(string[] args)
    {
        var inicio = StartUp();

        if (!string.IsNullOrWhiteSpace(inicio.Message))
        {
            Console.WriteLine(inicio.Message);
            return;
        }

        Console.ReadLine();

        await new InicializacaoController().StartProcess();
    }

    private static ValidationResult StartUp()
    {
        var resultado = new ValidationResult();

        try
        {
            AppParams.LoadAppParams();

            foreach (var arquivo in ArquivosConstantes.ArquivosNecessarios)
            {
                var entidadeArquivo = new Arquivo(arquivo, "txt");

                if (entidadeArquivo.Invalido)
                    File.Create(entidadeArquivo.Caminho);
            }
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            resultado.Message = $"Falha ao inicializar aplicação. Retorno'{message}'.";
        }

        return resultado;
    }
}
