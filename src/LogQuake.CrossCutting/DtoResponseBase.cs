using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LogQuake.CrossCutting
{
    /// <summary>
    /// Classe base para criação de objetos response
    /// </summary>
    /// <remarks>
    /// Utilizada em Controllers, Domains e Domains Services.
    /// </remarks>        
    public class DtoResponseBase
    {
        private IList<Notification> notifications = new List<Notification>();

        /// <summary>
        /// Coleção de notificações
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Notification> Notifications {
            get => notifications.Count == 0 ? null : notifications; 
            protected set  => notifications = value; 
        }

        [JsonIgnore]
        public bool Success
        {
            get
            {
                return notifications == null ||
                    !notifications.Any() ||
                    notifications.Count == 0;
            }
        }

        /// <summary>
        /// Obter as descrição do Enum da notificação
        /// </summary>
        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }

        /// <summary>
        /// Adiciona notificação a coleção de notificações
        /// </summary>
        /// <param name="notification">objeto de notificação</param>
        public void AddNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            notifications.Add(notification);
        }

        /// <summary>
        /// Adiciona notificação a coleção de notificações
        /// </summary>
        /// <param name="notification">objeto de notificação</param>
        /// <param name="userMessage">mensagem amigável destinada ao usuário</param>
        public void AddNotification(Enum notification, string userMessage)
        {
            Notification _notification = new Notification();
            _notification.ErrorCode = Convert.ToInt32(notification);
            _notification.Description = GetDescription((Notifications)notification);
            _notification.UserMessage = userMessage;

            AddNotification(_notification);
        }

        /// <summary>
        /// Adiciona notificação a coleção de notificações
        /// </summary>
        /// <param name="notification">objeto de notificação</param>
        /// <param name="userMessage">mensagem amigável destinada ao usuário</param>
        /// <param name="developerMessage">mensagem destinada ao desenvolvedor</param>
        public void AddNotification(Enum notification, string userMessage, string developerMessage)
        {
            Notification _notification = new Notification();
            _notification.ErrorCode = Convert.ToInt32(notification);
            _notification.Description = GetDescription((Notifications)notification);
            _notification.UserMessage = userMessage;
            _notification.DeveloperMessage = developerMessage;

            AddNotification(_notification);
        }

        /// <summary>
        /// Adiciona notificação a coleção de notificações
        /// </summary>
        /// <param name="notification">objeto de notificação</param>
        /// <param name="userMessage">mensagem amigável destinada ao usuário</param>
        /// <param name="ex">objeto de Exception</param>
        public void AddNotification(Enum notification, string userMessage, Exception ex)
        {
            Notification _notification = new Notification();
            _notification.ErrorCode = Convert.ToInt32(notification);
            _notification.Description = GetDescription((Notifications)notification);
            _notification.UserMessage = userMessage;
            _notification.DeveloperMessage = ex.ToString();

            AddNotification(_notification);
        }

        /// <summary>
        /// Adiciona notificação a coleção de notificações
        /// </summary>
        /// <param name="notification">objeto de notificação</param>
        /// <param name="ex">objeto de Exception</param>
        public void AddNotification(Enum notification, Exception ex)
        {
            string erro =
                "Exception type " + ex.GetType() + Environment.NewLine +
                "Exception message: " + ex.Message + Environment.NewLine +
                "Stack trace: " + ex.StackTrace + Environment.NewLine;
            if (ex.InnerException != null)
            {
                erro += "---BEGIN InnerException--- " + Environment.NewLine +
                           "Exception type " + ex.InnerException.GetType() + Environment.NewLine +
                           "Exception message: " + ex.InnerException.Message + Environment.NewLine +
                           "Stack trace: " + ex.InnerException.StackTrace + Environment.NewLine +
                           "---END Inner Exception";
            }

            Notification _notification = new Notification();
            _notification.ErrorCode = Convert.ToInt32(notification);
            _notification.Description = GetDescription((Notifications)notification);
            _notification.DeveloperMessage = erro;
            _notification.UserMessage = _notification.Description;

            AddNotification(_notification);
        }
    }
}
