﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using winCBCS.utility;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace winCBCS.faculty
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        string faculty_id;
       static string course_code;
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckCookies();
            
            if (!IsPostBack)
            {
                alert_error.Visible = false;
                alert_success.Visible = false;
                LoadCurriculum();
                
                LoadList();
            }
        }
        private void CheckCookies()
        {
            HttpCookie ck = Request.Cookies["FacultyCookie"];
            if (ck != null)
            {
                faculty_name.InnerHtml = ck["facultyName"].ToString();
                faculty_id = ck["facultyId"].ToString();
            }
            else
            {
                Response.Redirect("../logout.aspx");
            }
        }
        private void LoadList()
        {
            try
            {
                dataSubjectChoice.DataSource = DBConnection.GetDataTable("SELECT * FROM faculty_choice natural join timetable_subject where (faculty_id='"+faculty_id+"')");
                dataSubjectChoice.DataBind();
            }
            catch (Exception)
            {
            }
        }
        private void LoadAcademicYear()
        {
            try
            {
                drdAcademicYear.DataSource = DBConnection.GetDataTable("Select distinct course_academic_year from timetable_course where ");
                drdAcademicYear.DataTextField = "course_academic_year";
                drdAcademicYear.DataValueField = "course_academic_year";
                drdAcademicYear.DataBind();
                drdAcademicYear.Items.Insert(0, "");
            }
            catch (Exception)
            {

            }
        }

        protected void drdAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                drdSemester.DataSource = DBConnection.GetDataTable("select distinct course_academic_sem from timetable_course where program_curriculum='" + drpCurriculum.SelectedItem.ToString() + "' and course_name='" + drdCourseName.SelectedItem.ToString() + "' and course_academic_year='"+drdAcademicYear.SelectedItem.ToString()+"'");
                drdSemester.DataTextField = "course_academic_sem";
                drdSemester.DataValueField = "course_academic_sem";
                drdSemester.DataBind();
                drdSemester.Items.Insert(0, "");
            }
            catch (Exception)
            {
                
               
            } 
        }

        protected void drdCourseName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                drdAcademicYear.DataSource = DBConnection.GetDataTable("select distinct course_academic_year from timetable_course where program_curriculum='" + drpCurriculum.SelectedItem.ToString() + "' and course_name='" + drdCourseName.SelectedItem.ToString() + "' ");
                drdAcademicYear.DataTextField = "course_academic_year";
                drdAcademicYear.DataValueField = "course_academic_year";
                drdAcademicYear.DataBind();
                drdAcademicYear.Items.Insert(0, "");
            }
            catch (Exception)
            {
                
            }
            
        }

        protected void drdSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                drdSubjetName.DataSource = DBConnection.GetDataTable("select  * from timetable_subject where (course_curriculum='" + drpCurriculum.SelectedItem.ToString() + "' and course_name ='" + drdCourseName.SelectedItem.ToString() + "' and academic_year='" + drdAcademicYear.SelectedItem.ToString() + "' and subject_semester='"+drdSemester.SelectedItem.ToString()+"')");
                drdSubjetName.DataTextField = "subject_name";
                drdSubjetName.DataValueField = "subject_id";
                drdSubjetName.DataBind();
                drdSubjetName.Items.Insert(0, "");
            }
            catch (Exception)
            {

            }
        }

        protected void btnAddSubject_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["cbcs_connection"].ConnectionString);
            MySqlCommand cmd = new MySqlCommand("insert into faculty_choice ( faculty_id, subject_id, priority, exprience,course_code) values( ?faculty_id, ?subject_id, ?priority, ?exprience, ?course_code)", con);
            cmd.Parameters.AddWithValue("?faculty_id", faculty_id);
            cmd.Parameters.AddWithValue("?subject_id", drdSubjetName.SelectedValue);
            cmd.Parameters.AddWithValue("?priority", drdProirity.Text);
            cmd.Parameters.AddWithValue("?exprience", txtSubExperiance.Text);
            cmd.Parameters.AddWithValue("?course_code", course_code);
            
            try
            {
                con.Open();
                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                {
                    alert_success.Visible = true;
                    success_msg.InnerHtml = "Choice inserted Successful";
                    LoadList();
                }
                else
                {
                    alert_error.Visible = true;
                }
                con.Close();
            }
            catch (Exception ee)
            {
                alert_error.Visible = true;
                error_msg.InnerHtml = ee.Message;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        protected void dataSubjectChoice_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "data_delete")
            {
                DeleteChoice(e.CommandArgument.ToString());
            }
        }

        private void DeleteChoice(string p)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["cbcs_connection"].ConnectionString);
            MySqlCommand cmd = new MySqlCommand("delete from faculty_choice where choice_id='" + p + "'", con);

            try
            {
                con.Open();
                int res = cmd.ExecuteNonQuery();
                con.Close();
                if (res > 0)
                {
                    alert_success.Visible = true;
                    success_msg.InnerHtml = "Choice deleted Successful";
                }
                else
                {
                    alert_error.Visible = true;
                }
            }
            catch (Exception ee)
            {
                alert_error.Visible = true;
                error_msg.InnerHtml = ee.Message;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
            LoadList();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSubExperiance.Text = "";
            drdAcademicYear.SelectedIndex = -1;
            drdCourseName.SelectedIndex = -1;
            drdSemester.SelectedIndex = -1;
            drdProirity.SelectedIndex = -1;
            drdSubjetName.SelectedIndex = -1;

        }

        protected void drdSubjetName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = DBConnection.GetDataRow("select * from timetable_subject where subject_id ='"+drdSubjetName.SelectedValue+"'");
                if (dr!=null)
                {
                    course_code = dr["subject_code"].ToString();
                }
            }
            catch (Exception)
            {

            }
        }

        protected void drpCurriculum_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCourse();
        }

        private void LoadCourse()
        {
            try
            {
                drdCourseName.DataSource = DBConnection.GetDataTable("select distinct course_name from timetable_course where program_curriculum='"+drpCurriculum.SelectedItem.ToString()+"' ");
                drdCourseName.DataTextField = "course_name";
                drdCourseName.DataValueField = "course_name";
                drdCourseName.DataBind();
                drdCourseName.Items.Insert(0, "");
            }
            catch (Exception)
            {


            } 
        }
        private void LoadCurriculum()
        {
            try
            {
                drpCurriculum.DataSource = DBConnection.GetDataTable("select distinct program_curriculum from timetable_course");
                drpCurriculum.DataTextField = "program_curriculum";
                drpCurriculum.DataValueField = "program_curriculum";
                drpCurriculum.DataBind();
                drpCurriculum.Items.Insert(0, "");
            }
            catch (Exception)
            {

            }
            
        }
    }
}