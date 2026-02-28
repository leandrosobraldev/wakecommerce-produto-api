namespace WakeCommerce.API.Logging;

public static class LogsPageHtml
{
    public const string Content = """
        <!DOCTYPE html>
        <html><head><meta charset="utf-8"><title>Logs da API</title>
        <style>body{font-family:Consolas,monospace;margin:1rem;background:#1e1e1e;color:#d4d4d4;}
        table{border-collapse:collapse;width:100%;} th,td{border:1px solid #444;padding:6px 10px;text-align:left;}
        th{background:#333;} tr:nth-child(even){background:#252525;} .nivel-Warning{color:#dcdcaa;}
        .nivel-Error{color:#f48771;} .nivel-Information{color:#4ec9b0;} .nivel-Debug{color:#9cdcfe;}
        h1{color:#fff;} button{margin:8px 0;padding:8px 16px;cursor:pointer;}</style>
        </head><body>
        <h1>Logs da API</h1>
        <button onclick="location.reload()">Atualizar</button>
        <table><thead><tr><th>Data/Hora</th><th>Nível</th><th>Requisição</th><th>Categoria</th><th>Mensagem</th><th>Exceção</th></tr></thead>
        <tbody id="tbody"></tbody></table>
        <script>
        fetch('/api/v1/logs').then(r=>r.json()).then(entries=>{
            const tb=document.getElementById('tbody');
            tb.innerHTML=entries.map(e=>'<tr><td>'+e.dataHora+'</td><td class="nivel-'+e.nivel+'">'+e.nivel+'</td><td>'+e.metodo+' '+e.caminho+'</td><td>'+e.categoria+'</td><td>'+e.mensagem+'</td><td>'+e.excecao+'</td></tr>').join('');
        }).catch(err=>document.getElementById('tbody').innerHTML='<tr><td colspan="6">Erro ao carregar: '+err+'</td></tr>');
        </script></body></html>
        """;
}
