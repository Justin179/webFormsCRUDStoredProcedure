using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace asp.netcrud
{
    public partial class Contact : Page
    {

        SqlConnection sqlCon = new SqlConnection(@"data source=112953N1\SQLEXPRESS;initial catalog=EntityFrameworkTest;integrated security=True;");
        // Data Source=(local)\sqle2012;Initial Catalog=ASPCRUD;Integrated Security=true;
        // data source=112953N1\SQLEXPRESS;initial catalog=EntityFrameworkTest;integrated security=True;
        protected void Page_Load(object sender, EventArgs e)
        {
            // 第一次進來才執行
            if (!IsPostBack)
            {
                btnDelete.Enabled = false;
                FillGridView();
            }
        }
        // 新增修改
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            // 連結stored procedure
            SqlCommand sqlCmd = new SqlCommand("ContactCreateOrUpdate", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            // 塞參數 
            sqlCmd.Parameters.AddWithValue("@ConatctID", (hfContactID.Value == "" ? 0 : Convert.ToInt32(hfContactID.Value))); // 若為0則為新增
            sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            // 執行
            sqlCmd.ExecuteNonQuery();
            sqlCon.Close();

            // 從隱藏欄位取得id
            string contactID = hfContactID.Value;
            Clear();
            if (contactID == "")
                lblSuccessMessage.Text = "Saved Successfully";
            else
                lblSuccessMessage.Text = "Updated Successfully";
            
            FillGridView();
        }
        // 顯示資料
        void FillGridView()
        {
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewAll", sqlCon);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            sqlCon.Close();
            gvContact.DataSource = dtbl;
            gvContact.DataBind();

        }

        // 刪除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();

            SqlCommand sqlCmd = new SqlCommand("ContactDeleteByID", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@ContactID", Convert.ToInt32(hfContactID.Value));
            sqlCmd.ExecuteNonQuery();
            sqlCon.Close();
            // 先清空欄位
            Clear();
            // 再填值入欄位
            FillGridView();
            lblSuccessMessage.Text = "Deleted Successfully";
        }
        
        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
        // 清空欄位
        public void Clear()
        {
            hfContactID.Value = "";
            txtName.Text = txtMobile.Text = txtAddress.Text = "";
            lblSuccessMessage.Text = lblErrorMessage.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        protected void lnk_OnClick(object sender, EventArgs e)
        {
            // 這一行取得該row id
            int contactID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
            SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewByID", sqlCon);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.AddWithValue("@ContactID", contactID);
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            sqlCon.Close();

            // 把id塞到隱藏欄位
            hfContactID.Value = contactID.ToString();
            // 把該row的值帶回form
            txtName.Text = dtbl.Rows[0]["Name"].ToString();
            txtMobile.Text = dtbl.Rows[0]["Mobile"].ToString();
            txtAddress.Text = dtbl.Rows[0]["Address"].ToString();
            btnSave.Text = "Update";
            btnDelete.Enabled = true;
        }
    }
}