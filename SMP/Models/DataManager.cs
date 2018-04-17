using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMP.Models.Repositoryes;

namespace SMP.Models
{
    public class DataManager
    {
        private ModelContainer cont;
        public ProjectRepository projectRepository;

        public DataManager()
        {
            cont = new ModelContainer();
            projectRepository = new ProjectRepository(cont);
        }
    }
}