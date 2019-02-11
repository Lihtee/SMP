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
        public PersonRepository personRepository;
        public ProjectRepository projectRepository;
        public TeamRepository teamRepository;

        public DataManager()
        {
            cont = new ModelContainer();
            personRepository = new PersonRepository(cont);
            projectRepository = new ProjectRepository(cont);
            teamRepository = new TeamRepository(cont);
        }
    }
}