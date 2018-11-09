
namespace LogQuake.CrossCutting.Cache
{
    public class Cache
    {
        /// <summary>
        /// Objeto utilizado par controlar leitura e gravação do Cache
        /// </summary>
        public static object lockCache = new object();
    }
}
