using RainbowWine.Services.DO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RainbowWine.Services.DL
{
    public class UserTypeDL
    {
        private SqlConnection con;
        //To Handle connection related activities
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["RainbowConnection"].ToString();
            con = new SqlConnection(constr);

        }
        public bool AddEmployee(UserTypeDO obj)
        {

            connection();
            SqlCommand com = new SqlCommand("UserType_Ins", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@@UserTypeName", obj.UserTypeName);
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {

                return true;

            }
            else
            {

                return false;
            }
        }
        //public IList<UserTypeDO> GetUserType()
        //{

        //    connection();
        //    SqlCommand com = new SqlCommand("UserType_Sel", con);
        //    com.CommandType = CommandType.StoredProcedure;
        //    con.Open();
        //    int i = com();
        //    con.Close();
        //    if (i >= 1)
        //    {

        //        return true;

        //    }
        //    else
        //    {

        //        return false;
        //    }
        //}
    }
}