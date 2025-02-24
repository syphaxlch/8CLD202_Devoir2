using Azure.Storage.Blobs;  // Nécessaire pour interagir avec Azure Blob Storage

namespace MVC.Business
{
    // Le BlobController gère l'upload des fichiers dans Azure Blob Storage
    public class BlobController
    {
        private readonly BlobServiceClient _blobServiceClient;  // Client pour interagir avec Blob Storage
        private readonly string _containerName = "images";  // Nom du conteneur où les fichiers seront stockés
        private readonly ILogger<BlobController> _logger;  // Journalisation des erreurs ou autres événements

        // Le constructeur prend un BlobServiceClient pour interagir avec Azure Blob Storage et un logger pour suivre les événements
        public BlobController(BlobServiceClient blobServiceClient, ILogger<BlobController> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        // La méthode PushImageToBlob permet de télécharger l'image dans Azure Blob Storage
        public async Task<string> PushImageToBlob(IFormFile file, Guid imageGuid)
        {
            if (file == null || file.Length == 0)  // Vérifie si le fichier est valide
                throw new Exception("Aucun fichier fourni");

            try
            {
                // Récupère le client pour le conteneur blob où les fichiers seront stockés
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                // Si le conteneur n'existe pas, il est créé
                await containerClient.CreateIfNotExistsAsync();

                // Crée un nom unique pour le fichier (en utilisant le Guid et l'extension du fichier)
                var blobClient = containerClient.GetBlobClient(imageGuid.ToString() + Path.GetExtension(file.FileName));

                // Ouvre le fichier et l'upload dans le blob
                using (var stream = file.OpenReadStream())
                {
                    // Si un fichier avec ce nom existe déjà, il sera remplacé
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                // Retourne l'URL d'accès au fichier téléchargé
                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                // Si une erreur survient, elle est loguée et une exception est lancée
                _logger.LogError(ex, "Erreur lors de l'upload du fichier vers Azure Blob Storage");
                throw new Exception("Une erreur est survenue lors de l'upload de l'image.");
            }
        }
    }
}
