using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Amido.Testing.Saml2.Certificates;
using Amido.Testing.Saml2.Models;
using Amido.Testing.Saml2.TokenGenerator.Configuration;

namespace Amido.Testing.Saml2.TokenGenerator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "SAML 2.0 Generator";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("SAML 2.0 Generator");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();

            if (args.Any(x => x.Contains("?")) || args.Any(x => x.ToLower().Contains("help")))
            {
                WriteHelp();
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Loading configuration...");
                Console.ResetColor();

                X509Certificate2 certificate;

                var thumbprint = ConfigurationManager.AppSettings["Thumbprint"].ToString(CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(thumbprint))
                {
                    WriteToConsole("Thumbprint", thumbprint);

                    var storeName = ConfigurationManager.AppSettings["StoreName"];
                    WriteToConsole("StoreName", storeName);

                    var storeLocation = ConfigurationManager.AppSettings["StoreLocation"];
                    WriteToConsole("StoreLocation", storeLocation);

                    certificate = CertificateHelper.FindByThumbprint(thumbprint, GetStoreName(storeName), GetStoreLocation(storeLocation));
                    if (certificate != null)
                    {
                        Console.WriteLine("Certificate loaded successfully");
                    }
                }
                else
                {
                    var certificatePath = ConfigurationManager.AppSettings["CertificateLocation"];
                    WriteToConsole("Certificate path", certificatePath);

                    var certificatePassword = ConfigurationManager.AppSettings["CertificatePassword"];
                    WriteToConsole("Certificate password", certificatePassword);

                    certificate = CertificateHelper.FindFromFile(certificatePath, certificatePassword);
                    if (certificate != null)
                    {
                        Console.WriteLine("Certificate loaded successfully");
                    }
                }

                var notBeforeDate = DateTime.Parse(ConfigurationManager.AppSettings["NotBeforeDate"]);
                WriteToConsole("Not before date", notBeforeDate.ToShortDateString());

                var notOnOrAfterDate = DateTime.Parse(ConfigurationManager.AppSettings["NotOnOrAfterDate"]);
                WriteToConsole("Not on or after date", notOnOrAfterDate.ToShortDateString());

                var issuer = ConfigurationManager.AppSettings["Issuer"];
                WriteToConsole("Issuer", issuer);

                var audience = ConfigurationManager.AppSettings["Audience"];
                WriteToConsole("Audience", audience);

                var nameIdentifier = ConfigurationManager.AppSettings["NameIdentifier"];
                WriteToConsole("Name Identifier", nameIdentifier);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Loading certificate...");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Creating SAML 2.0 token");
                Console.ResetColor();
                var tokenFactory = new TokenFactory();

                var token = tokenFactory.CreateSaml2BearerToken(new Saml2TokenProperties
                {
                    NameIdentifier = nameIdentifier,
                    Audience = audience,
                    Issuer = issuer,
                    X509Certificate2 = certificate,
                    NotBeforeDate = notBeforeDate,
                    NotOnOrAfterDate = notOnOrAfterDate,
                    Claims = GetClaims()
                });

                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("SAML 2.0 token string generated");
                }
                else
                {
                    throw new Exception("SAML 2.0 token string was empty... there must be a bug");
                }

                if (bool.Parse(ConfigurationManager.AppSettings["Base64Token"]))
                {
                    token = EncodeBase64(token);
                    Console.WriteLine("Base64 encoded SAML 2.0 token string");
                }

                if (bool.Parse(ConfigurationManager.AppSettings["UrlEncodeToken"]))
                {
                    token = HttpUtility.UrlEncode(token);
                    Console.WriteLine("Url encoded SAML 2.0 token");
                }

                if (token == null)
                {
                    throw new Exception("SAML 2.0 token string was empty... there must be a bug");
                }

                Clipboard.SetText(token);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SAML 2.0 token added to clipboard");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oops, there was an error");
                Console.ResetColor();
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }
        }

        private static void WriteToConsole(string key, string value, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("{0}:", key);
            Console.ResetColor();
            Console.WriteLine(value);
        }

        private static StoreName GetStoreName(string storeName)
        {
            switch(storeName.ToLower())
            {
                case "my":
                    return StoreName.My;
                case "addressbook":
                    return StoreName.AddressBook;
                case "authroot":
                    return StoreName.AuthRoot;
                case "certificateauthority":
                    return StoreName.CertificateAuthority;
                case "root":
                    return StoreName.Root;
                case "trustedpeople":
                    return StoreName.TrustedPeople;
                case "trustedpublisher":
                    return StoreName.TrustedPublisher;
            }

            throw new ArgumentException("The storename " + storeName + " is not supported.");
        }

        private static StoreLocation GetStoreLocation(string storeLocation)
        {
            switch (storeLocation.ToLower())
            {
                case "localmachine":
                    return StoreLocation.LocalMachine;
                case "currentuser":
                    return StoreLocation.CurrentUser;
            }
            throw new ArgumentException("The storeLocation " + storeLocation + " is not supported.");
        }

        private static void WriteHelp()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Help");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("The values for generating the SAML 2.0 token are set within the Saml2Generator.exe.config");
            Console.WriteLine("===============================================================================");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("If using a certificate on the file system");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CertificateLocation");
            Console.ResetColor();
            Console.WriteLine("This is the file path to the certificate .pfx file");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CertificatePassword");
            Console.ResetColor();
            Console.WriteLine("This is password of the certificate .pfx file");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("If using a certificate on the certificate store");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Thumprint");
            Console.ResetColor();
            Console.WriteLine("This is the thumbprint of the signing certificate");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Storename");
            Console.ResetColor();
            Console.WriteLine("This is the certificate store name such as 'My'");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("StoreLocation");
            Console.ResetColor();
            Console.WriteLine("This is the certificate store location such as 'CurrentUser'");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NotBeforeDate");
            Console.ResetColor();
            Console.WriteLine("This is the minimum date when the token can be used");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NotOnOrAfterDate");
            Console.ResetColor();
            Console.WriteLine("This is the date when the token becomes invalid");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Issuer");
            Console.ResetColor();
            Console.WriteLine("This is name of the token issuer");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Audience");
            Console.ResetColor();
            Console.WriteLine("This is name of the token audience");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NameIdentifier");
            Console.ResetColor();
            Console.WriteLine("This is name identifier claim");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Claims Collection");
            Console.ResetColor();

            Console.WriteLine("Multiple claims can be added to the SAML 2.0 assertion statements");

            Console.WriteLine("The claims are added as name-value pairs for example:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("<claim name=\"http://mycustomclaim\" value=\"myvalue\" />");
        }

        private static Dictionary<string, string> GetClaims()
        {
            var claimsSection = (ClaimsConfigurationSection)ConfigurationManager.GetSection("claimsList");

            return claimsSection
                .Claims
                .Cast<ClaimConfigurationElement>()
                .ToDictionary(claim => claim.Name, claim => claim.Value);
        }

        private static string EncodeBase64(string token)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string encodedToken = Convert.ToBase64String(encoding.GetBytes(token));
            return encodedToken;
        }
    }
}
