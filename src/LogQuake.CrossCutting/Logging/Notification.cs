using Newtonsoft.Json;
using System.ComponentModel;

namespace LogQuake.CrossCutting
{
    /// <summary>
    /// Estrutura do objeto de retorno no respone, quando houver alguma notificaçãoao usuário ou as camadas superiores do sistema.
    /// </summary>
    public class Notification
    {
        internal Notification()
        {
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DeveloperMessage { get; set; }

        public string UserMessage { get; set; }
    }

    /// <summary>
    /// Repositório de todas as notificações do sistema. Para cada nova notificação deve ser inserido uma descrição, que será utilizada no reponse.
    /// </summary>
    public enum Notifications
    {
        [Description("Erro inesperado.")]
        ErroInesperado = 0,

        [Description("Objeto Nulo.")]
        ObjetoNulo = 1,

        [Description("Nenhum item encontrado com os parâmetros informados.")]
        ItemNaoEncontrado = 2

    }

}
