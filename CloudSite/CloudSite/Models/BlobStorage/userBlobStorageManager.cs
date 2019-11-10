﻿using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloudSite.Models.BlobStorage
{
    class UserBlobStorageManager
    {
        private string _userId;
        private CloudBlobClient _connection;

        public CloudBlobContainer userContainer;

        public UserBlobStorageManager(CloudBlobClient connection, string userId)
        {
            _connection = connection;
            _userId = userId;

            SelectConteinerUser();
        }

        public void ChangeUserSelected(string userId)
        {
            _userId = userId;
            SelectConteinerUser();
        }

        private void SelectConteinerUser()
        {
            userContainer = _connection.GetContainerReference(_userId);
            userContainer.CreateIfNotExistsAsync().Wait();
        }

        public void RemovePhotoFromBlobStorage(List<string> photosName)
        {
            foreach (string name in photosName)
            {
                try
                {
                    var blobToDelete = userContainer.GetBlockBlobReference(name);
                    blobToDelete.DeleteIfExists();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void AddPhotoToUserContainer(Stream photo, string photoName)
        {
            if (userContainer == null)
                throw new ArgumentException("Container is note define", "NullContainer");
            else if (userContainer.Name != _userId)
                throw new ArgumentException("Container don't mach the user", "NoMatchBetweenContainerAndUser");

            CloudBlockBlob cBlob = userContainer.GetBlockBlobReference(photoName);
            
            cBlob.UploadFromStream(photo);
            photo.Close();
        }

        // References: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-service-sas-create-dotnet
        public string GetContainerSasUri(int minutesToAdd = 1)
        {
            string sasContainerToken;
            int timeDifferencesInMinutes = (DateTime.Now.Hour - DateTime.UtcNow.Hour) * 60;

            SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(minutesToAdd + timeDifferencesInMinutes),
                Permissions = SharedAccessBlobPermissions.Read
            };

            sasContainerToken = userContainer.GetSharedAccessSignature(adHocPolicy, null);
            
            return sasContainerToken;
        }

        public string GetLinkForSharePhoto(DateTime expireDate)
        {
            int totalNumberOfMinutesToAdd = (int)(expireDate - DateTime.Now).TotalMinutes;
            return GetContainerSasUri(totalNumberOfMinutesToAdd);
        }
    }
}
