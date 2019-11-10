﻿using System;
using MongoDB.Driver;

namespace CloudSite.Models.MoongoDB
{
    class DBManager
    {
        private const string HOST = Variables.HOST_FOR_MONGODB;
        private const string PORT = Variables.PORT_FOR_MONGODB;
        private const string DATABASE_NAME = Variables.NAME_OF_DATABASE_IN_MONGODB;
        private const string DATABASE_ADMINISTRATOR_NAME = Variables.USER_FOR_AUTHENTICATION_MONGODB;
        private const string DATABASE_ADMINISTRATOR_PASSWORD = Variables.PASSWORD_FOR_AUTHENTICATION_MONGODB;

        private MongoClient databaseConnection = null;
        private IMongoDatabase database;

        public PhotoManager photoManager { get; set; }
        public UserManager userManager { get; set; }

        public DBManager()
        {
            ConnectionToMongoDB();
            photoManager = new PhotoManager(database);
            userManager = new UserManager(database);
        }
        private void ConnectionToMongoDB()
        {
            try
            {
                string connectionString = string.Format(
                "mongodb://{0}:{1}@{2}:{3}/{4}",
                DATABASE_ADMINISTRATOR_NAME,
                DATABASE_ADMINISTRATOR_PASSWORD,
                HOST,
                PORT,
                DATABASE_NAME
                );

                databaseConnection = new MongoClient(connectionString);
                database = databaseConnection.GetDatabase(DATABASE_NAME);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
