using System.Net;
using System.Net.Mail;

namespace PetAppointmentReservationSystem.Helpers
{
    public static class EmailHelper
    {
        // TODO: replace with your real SMTP provider details before going live.
        private const string SmtpHost = "smtp.yourprovider.com";
        private const int SmtpPort = 587;
        private const string SmtpUsername = "your-smtp-username";
        private const string SmtpPassword = "your-smtp-password";
        private const string FromAddress = "noreply@petconnect.com";

        public static bool SendAppointmentConfirmation(string toEmail, string petName, string serviceName, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                return false;
            }

            try
            {
                using var smtpClient = new SmtpClient(SmtpHost)
                {
                    Port = SmtpPort,
                    Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(FromAddress, "PetConnect"),
                    Subject = "PetConnect Appointment Confirmation",
                    Body = $"Hi,\n\nYour appointment for {petName} ({serviceName}) is confirmed for {date:f}.\n\nThank you for choosing PetConnect!",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Don't let a failed email block the booking flow — just log it.
                System.Diagnostics.Debug.WriteLine("Email send failed: " + ex.Message);
                return false;
            }
        }
    }
}