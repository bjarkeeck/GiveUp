﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace GiveUp.Classes.Db
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base(@"Data Source=ns1.lilac.arvixe.com;Initial Catalog=minji;Persist Security Info=True;User ID=minji;Password=minjiminji")
        {

        }

        [Obsolete("Ik brug den her metode særlig tit!")]
        public static void ReCreateLeveldataForEachUser()
        {
            DataContext db = DataContext.Current;

            foreach (var item in db.Levels.ToList())
            {
                db.Levels.Remove(item);
            }

            db.SaveChanges();

            foreach (User user in db.Users)
            {
                var levelDir = new DirectoryInfo("../../../Content/Levels/");

                foreach (DirectoryInfo item in levelDir.GetDirectories())
                {
                    int levelName = int.Parse(item.Name);

                    foreach (FileInfo f in item.GetFiles())
                    {
                        if (f.Extension.ToLower().Contains("txt"))
                        {
                            int subLevel = int.Parse(f.Name.ToLower().Replace(".txt", ""));
                            if (db.Levels.Any(x => x.LevelId == levelName && x.SubLevelId == subLevel && x.User.Username == user.Username) == false)
                            {
                                Level level = new Level()
                                {
                                    LevelId = levelName,
                                    SubLevelId = subLevel,
                                    User = user
                                };
                                db.Levels.Add(level);
                            }
                        }
                    }
                }
            }
            db.SaveChanges();
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Level> Levels { get; set; }



        private static DataContext current;
        public static DataContext Current
        {
            get
            {
                if (current == null)
                {
                    current = new DataContext();
                }
                return current;
            }
            set
            {
                current = value;
            }

        }

    }
}
