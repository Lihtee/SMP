using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.Models.Repositoryes
{
    public class AddictionRepository
    {
        static ModelContainer cont = new ModelContainer();

        public AddictionRepository(ModelContainer _cont)
        {
            cont = _cont;
        }

        /// <summary>
        /// Возвращает все зависимости
        /// </summary>
        /// <returns>Список зависимостей</returns>
        public List<Addiction> GetAddictions()
        {
            return cont.Addiction.OrderBy(p => p.Id).ToList();
        }

        /// <summary>
        /// Возвращает зависимость по Id зависимости
        /// </summary>
        /// <param name="Id">Id зависимости</param>
        /// <returns>Зависимость</returns>
        public Addiction GetAddictionById(int Id)
        {
            return cont.Addiction.SingleOrDefault(p => p.Id == Id);
        }

        /// <summary>
        /// Возвращает зависимость по Id предшествующего проекта
        /// </summary>
        /// <param name="Id">Id предшествующего проекта</param>
        /// <returns>Список зависимостей</returns>
        public List<Addiction> GetAddictionsByLastProjectID(int Id)
        {
            return cont.Addiction.OrderBy(p => p.lastProject.IdProject == Id).ToList();
        }

        /// <summary>
        /// Возвращает зависимость по Id следующего проекта
        /// </summary>
        /// <param name="Id">Id следующего проекта</param>
        /// <returns>Список зависимостей</returns>
        public List<Addiction> GetAddictionsByNextProjectID(int Id)
        {
            return cont.Addiction.OrderBy(p => p.nextProject.IdProject == Id).ToList();
        }

        /// <summary>
        /// Добавляет зависимость в базу
        /// </summary>
        /// <param name="lastProjectId">Id предшествующего проекта</param>
        /// <param name="nextProjectId">Id следующего проекта</param>
        /// <returns>Добавленная зависимость</returns>
        public Addiction AddAddiction(int lastProjectId, int nextProjectId)
        {
            Addiction addiction = new Addiction
            {
                lastProject = cont.Project.SingleOrDefault(p => p.IdProject == lastProjectId),
                nextProject = cont.Project.SingleOrDefault(p => p.IdProject == nextProjectId),
            };
            cont.Addiction.Add(addiction);
            cont.SaveChanges();
            return addiction;
        }

        /// <summary>
        /// Изменяет зависимость по Id зависимости
        /// </summary>
        /// <param name="addictiontId">Id зависимости</param>
        /// <param name="lastProjectId">Id предшествующего проекта</param>
        /// <param name="nextProjectId">Id следующего проекта</param>
        /// <returns>Изменённая зависимость</returns>
        public Addiction EditAddiction(int addictiontId, int lastProjectId, int nextProjectId)
        {
            Addiction addictiont = GetAddictionById(addictiontId);

            addictiont.lastProject = cont.Project.SingleOrDefault(p => p.IdProject == lastProjectId);
            addictiont.lastProject = cont.Project.SingleOrDefault(p => p.IdProject == nextProjectId);
            
            cont.SaveChanges();
            return addictiont;
        }

        /// <summary>
        /// Удаляет зависимость по Id зависимости
        /// </summary>
        /// <param name="addictiontId">Id зависимости</param>
        public void DeleteAddiction(int addictiontId)
        {
            cont.Addiction.Remove(GetAddictionById(addictiontId));
            cont.SaveChanges();
        }
    }
}