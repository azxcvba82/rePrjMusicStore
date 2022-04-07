using Microsoft.AspNetCore.Http;
using Music_store_backend.Model.Auth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class UserMenu
    {
        public List<NGMenuItem> mainMenu { get; set; }
        public List<NGMenuItem> top1Menu { get; set; }

        public List<NGMenuItem> top2Menu { get; set; }

        public List<NGMenuItem> hiddenMenu { get; set; }

        public List<NGMenuItem> loginUserMenu { get; set; }

        public String top1MenuLabel { get; set; }
        public String top2MenuLabel { get; set; }

        public String homeRouterLink { get; set; }

        public static UserMenu GetUserMenu(AppSettings Util, UserPrincipal user, HttpContext httpContext)
        {
            if (user.IsInRole("SysAdmin") || user.IsInRole("SuperAdmin"))
            {
                UserMenu userMenu = GetMenuFromAdmin(Util, user);
                return userMenu;
            }
            return GetMenuFromUser(Util, user);
        }

        private static UserMenu GetMenuFromAdmin(AppSettings Util, UserPrincipal user)
        {
            UserMenu userMenu = null;
            userMenu = GetMenuAdmin(Util, user);
            return userMenu;
        }

        private static UserMenu GetMenuFromUser(AppSettings Util, UserPrincipal user)
        {
            UserMenu userMenu = null;
            userMenu = GetMenuUser(Util, user);
            return userMenu;
        }

        private static UserMenu GetMenuAdmin(AppSettings Util, UserPrincipal user)
        {
            UserMenu userMenu = new UserMenu();
            String role = "SysAdmin";
            userMenu.mainMenu = GetMenuList(Util, role);
            userMenu.homeRouterLink = "/pages/music-store/index";

            return userMenu;

        }

        private static UserMenu GetMenuUser(AppSettings Util, UserPrincipal user)
        {
            UserMenu userMenu = new UserMenu();
            String role = "NormalUser";
            userMenu.mainMenu = GetMenuList(Util, role);
            userMenu.homeRouterLink = "/pages/music-store/index";

            return userMenu;

        }

        public static List<NGMenuItem> GetMenuList(AppSettings Util, String roleId)
        {
            List<NGMenuItem> menuList = new List<NGMenuItem>();
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            String strSQL = "SELECT * FROM tMenu WITH (NOLOCK) " +
                            "WHERE fRole=@fRole ORDER BY fSequence";

            SqlDataAdapter myCommand = new SqlDataAdapter(strSQL, myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("fRole", roleId));
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "Menu");

            DataView dv = new DataView(ds.Tables["Menu"]);
            //dv.RowFilter = "parentMenuId is null";
            foreach (DataRowView r in dv)
            {

                NGMenuItem menuItem = new NGMenuItem
                {
                    menuId = (int)r["fId"],
                    label = Util.GetDBString(r["fLabel"]),
                    icon = Util.GetDBString(r["fIcon"]) == "" ? null : Util.GetDBString(r["fIcon"]),
                    routerlink = Util.GetDBString(r["fRouterlink"]) == "" ? null : Util.GetDBString(r["fRouterlink"]),
                    url = Util.GetDBString(r["fUrl"]) == "" ? null : Util.GetDBString(r["fUrl"]),
                };
                menuList.Add(menuItem);
            }

            return menuList;


        }
    }
}

