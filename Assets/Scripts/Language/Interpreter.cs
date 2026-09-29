using System;

public class Interpreter
{
    public string Executar(string codigo)
    {
        codigo = codigo.Trim();

        if (codigo.StartsWith("escrever(") && codigo.EndsWith(")"))
        {
            string texto = codigo.Substring(9, codigo.Length - 10);

            texto = texto.Trim('"');

            return texto;
        }

        return null;
    }
}