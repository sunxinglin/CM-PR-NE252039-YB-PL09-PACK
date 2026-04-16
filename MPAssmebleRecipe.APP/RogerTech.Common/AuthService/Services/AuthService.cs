using SqlSugar;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Common.AuthService.Model;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System;
using System.Linq;
using RogerTech.Common.AuthService;

namespace RogerTech.AuthService
{
    public class AuthService
    {
        #region User相关操作
        /// <summary>
        /// 通过工号获取用户ID
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public User GetUserByEmployeeId(string employeeId)
        {
            return DbContext.GetInstance().Queryable<User>().Where(x=>x.EmployeeId == employeeId).First();
        }

        public List<User> GetAllUsers()
        {
            return DbContext.UserDb.GetList();
        }

        public bool AddUser(User user, out string message)
        {
            message = string.Empty;
            var item = GetUserByEmployeeId(user.EmployeeId);
            if (item == null)
            {
                bool result = DbContext.UserDb.Insert(user);
                if (!result)
                {
                    message = $"添加用户:{user.UserName}失败";
                }
                return result;
            }
            else
            {
                message = $"用户:{user.UserName}已存在";
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            return DbContext.UserDb.Update(user);
        }

        public bool DeleteUser(int id)
        {
            try
            {
                DbContext.GetInstance().Ado.BeginTran();
                // 删除用户角色关联
                DbContext.UserRoleDb.Delete(ur => ur.UserId == id);
                // 删除用户工位关联
                DbContext.UserWorkstationDb.Delete(uw => uw.UserId == id);
                // 删除用户
                var result = DbContext.UserDb.DeleteById(id);
                DbContext.GetInstance().Ado.CommitTran();
                return result;
            }
            catch
            {
                DbContext.GetInstance().Ado.RollbackTran();
                throw;
            }
        }
        #endregion

        #region Role相关操作
        public Role GetRoleById(int id)
        {
            return DbContext.RoleDb.GetById(id);
        }

        public List<Role> GetAllRoles()
        {
            return DbContext.RoleDb.GetList();
        }

        public List<Role> GetUserRoles(int userId)
        {
            return DbContext.GetInstance().Queryable<Role>()
                .InnerJoin<UserRole>((r, ur) => r.Id == ur.RoleId)
                .Where((r, ur) => ur.UserId == userId)
                .ToList();
        }

        public bool AddRole(Role role)
        {
            return DbContext.RoleDb.Insert(role);
        }

        public bool UpdateRole(Role role)
        {
            return DbContext.RoleDb.Update(role);
        }

        public bool DeleteRole(int id)
        {
            try
            {
                DbContext.GetInstance().Ado.BeginTran();
                // 删除角色菜单关联
                DbContext.RoleMenuDb.Delete(rm => rm.RoleId == id);
                // 删除用户角色关联
                DbContext.UserRoleDb.Delete(ur => ur.RoleId == id);
                // 删除角色
                var result = DbContext.RoleDb.DeleteById(id);
                DbContext.GetInstance().Ado.CommitTran();
                return result;
            }
            catch
            {
                DbContext.GetInstance().Ado.RollbackTran();
                throw;
            }
        }
        #endregion

        #region Menu相关操作
        public Menu GetMenuById(int id)
        {
            return DbContext.MenuDb.GetById(id);
        }

        public List<Menu> GetAllMenus()
        {
            return DbContext.MenuDb.GetList();
               
        }

        public List<Menu> GetUserMenus(int userId)
        {
            throw new NotImplementedException();
                
        }
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool AddMenu(Menu menu)
        {
            var item = DbContext.GetInstance().Queryable<Menu>().Where(x => x.Page == menu.Page && x.SubPage == menu.SubPage && x.ElementName == menu.ElementName).First();
            if (item == null)
            {
                int n = DbContext.GetInstance().Insertable<Menu>(menu).ExecuteCommand();
                return n == 1 ? true : false;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateMenu(Menu menu)
        {
            return DbContext.MenuDb.Update(menu);
        }

        //public bool DeleteMenu(int id)
        //{
        //    try
        //    {
        //        DbContext.GetInstance().Ado.BeginTran();
        //        // 删除角色菜单关联
        //        DbContext.RoleMenuDb.Delete(rm => rm.MenuId == id);
        //        // 删除子菜单
        //        DbContext.MenuDb.Delete(m => m.ParentId == id);
        //        // 删除菜单
        //        var result = DbContext.MenuDb.DeleteById(id);
        //        DbContext.GetInstance().Ado.CommitTran();
        //        return result;
        //    }
        //    catch
        //    {
        //        DbContext.GetInstance().Ado.RollbackTran();
        //        throw;
        //    }
        //}
        #endregion

        #region Workstation相关操作
        public Workstation GetWorkstationById(int id)
        {
            return DbContext.WorkstationDb.GetById(id);
        }

        public List<Workstation> GetAllWorkstations()
        {
            return DbContext.WorkstationDb.GetList();
        }

        public List<Workstation> GetUserWorkstations(int userId)
        {
            return DbContext.GetInstance().Queryable<Workstation>()
                .InnerJoin<UserWorkstation>((w, uw) => w.Id == uw.WorkstationId)
                .Where((w, uw) => uw.UserId == userId)
                .ToList();
        }

        public bool AddWorkstation(Workstation workstation)
        {
            return DbContext.WorkstationDb.Insert(workstation);
        }

        public bool UpdateWorkstation(Workstation workstation)
        {
            return DbContext.WorkstationDb.Update(workstation);
        }

        public bool DeleteWorkstation(int id)
        {
            return DbContext.WorkstationDb.DeleteById(id);
        }
        #endregion

        #region 权限分配操作
        public bool AssignUserRole(int userId, int roleId)
        {
            var userRole = new UserRole { UserId = userId, RoleId = roleId };
            return DbContext.UserRoleDb.Insert(userRole);
        }

        public bool RemoveUserRole(int userId, int roleId)
        {
            return DbContext.UserRoleDb.Delete(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        public bool UpdateRoleMenus(int roleId, List<int> menuIds)
        {
            try
            {
                DbContext.GetInstance().Ado.BeginTran();
                // 删除原有权限
                DbContext.RoleMenuDb.Delete(rm => rm.RoleId == roleId);
                // 添加新权限
                var roleMenus = menuIds.Select(menuId => new RoleMenu 
                { 
                    RoleId = roleId, 
                    MenuId = menuId 
                }).ToList();
                var result = DbContext.RoleMenuDb.InsertRange(roleMenus);
                DbContext.GetInstance().Ado.CommitTran();
                return result;
            }
            catch
            {
                DbContext.GetInstance().Ado.RollbackTran();
                throw;
            }
        }

        public List<RoleMenu> GetRoleMenusByRoleId(int roleId)
        {
            return DbContext.RoleMenuDb.GetList(rm => rm.RoleId == roleId);
        }

        public IEnumerable<Menu> GetRoleMenus(int id)
        {
            var items =  DbContext.GetInstance().Queryable<RoleMenu>().
                Where(x=>x.RoleId == id).ToList();
            var ids = items.Select(x=> x.MenuId).ToList();
            var menus = DbContext.GetInstance().Queryable<Menu>().Where(x => ids.Contains(x.Id)).ToList();
            return menus;
        }
        #endregion

        #region 角色管理关系操作
        public List<Role> GetManagedRoles(int managerRoleId)
        {
            return DbContext.GetInstance().Queryable<Role>()
                .InnerJoin<RoleManage>((r, rm) => r.Id == rm.ManagedRoleId)
                .Where((r, rm) => rm.ManagerRoleId == managerRoleId)
                .ToList();
        }

        public List<Role> GetManagerRoles(int managedRoleId)
        {
            return DbContext.GetInstance().Queryable<Role>()
                .InnerJoin<RoleManage>((r, rm) => r.Id == rm.ManagerRoleId)
                .Where((r, rm) => rm.ManagedRoleId == managedRoleId)
                .ToList();
        }

        public bool AssignRoleManager(int managerRoleId, int managedRoleId)
        {
            // 检查是否存在循环管理
            if (IsCircularManagement(managerRoleId, managedRoleId))
            {
                return false;
            }

            var roleManage = new RoleManage 
            { 
                ManagerRoleId = managerRoleId, 
                ManagedRoleId = managedRoleId,
                CreatedTime = DateTime.Now
            };
            return DbContext.GetInstance().Insertable(roleManage).ExecuteCommand() > 0;
        }

        public bool RemoveRoleManager(int managerRoleId, int managedRoleId)
        {
            return DbContext.GetInstance().Deleteable<RoleManage>()
                .Where(rm => rm.ManagerRoleId == managerRoleId && rm.ManagedRoleId == managedRoleId)
                .ExecuteCommand() > 0;
        }

        private bool IsCircularManagement(int managerRoleId, int managedRoleId)
        {
            // 检查是否形成循环管理
            var managedRoles = new HashSet<int>();
            return CheckCircularManagement(managedRoleId, managerRoleId, managedRoles);
        }

        private bool CheckCircularManagement(int currentRoleId, int targetRoleId, HashSet<int> visitedRoles)
        {
            if (currentRoleId == targetRoleId)
            {
                return true;
            }

            if (!visitedRoles.Add(currentRoleId))
            {
                return false;
            }

            var managedRoles = GetManagedRoles(currentRoleId);
            foreach (var role in managedRoles)
            {
                if (CheckCircularManagement(role.Id, targetRoleId, visitedRoles))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion









    }
} 