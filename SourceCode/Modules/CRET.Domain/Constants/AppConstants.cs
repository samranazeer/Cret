using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Constants
{
    public static class Constants
    {
        public static readonly string EmailBodyForCSR = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                </head>
                <body>

                Dear customer,
                <br/> <br/> 
                Thank you for reaching out and requesting access certificates for our UltraConnect application.<br/>
                Please find attached the QR code(s) containing the <strong>certificate token</strong>, each valid for one mobile device.<br/> <br/> 
 
                Please scan the QR code with the UltraConnect application (Certificates &#10140; Scan QR code).<br/>
                UltraConnect will then create a device-specific certificate signing request (csr) which needs to be sent to<br/> <br/> 
 
                <a href=""mailto:certificates.de@landisgyr.com"">certificates.de@landisgyr.com</a> for signing.<br/> <br/> 
 
                For a complete description of the process, please have a look at the attached document.<br/> <br/> <br/> 
 
 
                With best regards, mit freundlichen Grüßen
                <strong><p>Landis<span style=""color: #7AB800;"">&#10010;</span>Gyr GmbH</p></strong>
                Humboldtstraße 64<br/> 
                90459 Nuremberg<br/> 
                Germany<br/> <br/> <br/> 
                </body>
                </html>";


        public static readonly string EmailBodyForCertificate = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Empty HTML File</title>
                </head>
                <body>

                Dear customer,
                <br/> <br/> 
                Thank you for reaching out and requesting access certificates for our UltraConnect application.<br/>
                Please find attached the QR code(s) containing the <strong>signed certificate</strong>, each valid for one mobile device.<br/> <br/> 
                <strong>Important: </strong>The certificate is valid only for the exact device the csr has ben created with!<br/> <br/> 
                Please scan the QR code with the UltraConnect application (Certificates &#10140; Scan QR code).<br/>
                After successful import, the new certificate will be displayed and activated by default.
                <br/> <br/> 
                For a complete description of the process, please have a look at the attached document.<br/> <br/> <br/> 
                With best regards, mit freundlichen Grüßen

                <strong><p>Landis<span style=""color: #7AB800;"">&#10010;</span>Gyr GmbH</p></strong>
                
                Humboldtstraße 64<br/> 
                
                90459 Nuremberg<br/> 
                
                Germany<br/> <br/> <br/> 
                </body>
                </html>";
    }
}

