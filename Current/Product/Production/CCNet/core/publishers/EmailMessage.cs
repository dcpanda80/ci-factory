using System.Collections;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// This class encloses all the details related to a typical message needed by a 
	/// Email Publisher
	/// </summary>
	public class EmailMessage
	{
		private readonly IIntegrationResult result;
		private readonly EmailPublisher emailPublisher;

		public EmailMessage(IIntegrationResult result, EmailPublisher emailPublisher)
		{
			this.result = result;
			this.emailPublisher = emailPublisher;
		}

		public string Recipients
		{
			get
			{
				IDictionary recipients = new SortedList();
				AddRecipients(recipients, EmailGroup.NotificationType.Always);
				AddModifiers(recipients);
                AddForcer(recipients);

				if (BuildStateChanged(result))
				{
					AddRecipients(recipients, EmailGroup.NotificationType.Change);
				}

				if (this.result.Status == IntegrationStatus.Failure)
				{
					AddRecipients(recipients, EmailGroup.NotificationType.Failed);
                }

                if (this.result.Status == IntegrationStatus.Success)
                {
                    AddRecipients(recipients, EmailGroup.NotificationType.Success);
                }

				StringBuilder buffer = new StringBuilder();
				foreach (string key in recipients.Keys)
				{
					if (buffer.Length > 0) buffer.Append(", ");
					buffer.Append(key);
				}
				return buffer.ToString();
			}
		}

        private void AddForcer(IDictionary recipients)
        {
            if (result.IntegrationProperties.Contains("CCNetForcedBy"))
            {
                string username = result.IntegrationProperties["CCNetForcedBy"];
                EmailUser user = GetEmailUser(username);
                if (user != null)
                {
                    recipients[user.Address] = user;
                }
            }

        }

		private void AddModifiers(IDictionary recipients)
		{
			foreach (Modification modification in result.Modifications)
			{
				EmailUser user = GetEmailUser(modification.UserName);
				if (user != null)
				{
					recipients[user.Address] = user;
				}
			}
		}

		private void AddRecipients(IDictionary recipients, EmailGroup.NotificationType notificationType)
		{
			foreach (EmailUser user in emailPublisher.EmailUsers.Values)
			{
				EmailGroup group = GetEmailGroup(user.Group);
				if (group != null && group.Notification == notificationType)
				{
					recipients[user.Address] = user;
				}
			}
		}

		public string Subject
		{
			get
			{
				if (result.Status == IntegrationStatus.Success)
				{
					if (BuildStateChanged(result))
					{
						return string.Format("{0} {1} {2}", result.ProjectName, "Build Fixed: Build", result.Label);
					}
					else
					{
						return string.Format("{0} {1} {2}", result.ProjectName, "Build Successful: Build", result.Label);
					}
				}
				else
				{
					return string.Format("{0} {1}", result.ProjectName, "Build Failed");
				}
			}
		}

		private EmailUser GetEmailUser(string username)
		{
			if (username == null) return null;
			return (EmailUser) emailPublisher.EmailUsers[username];
		}

		private EmailGroup GetEmailGroup(string groupname)
		{
			return (EmailGroup) emailPublisher.EmailGroups[groupname];
		}

		private bool BuildStateChanged(IIntegrationResult result)
		{
			return result.LastIntegrationStatus != result.Status;
		}
	}
}