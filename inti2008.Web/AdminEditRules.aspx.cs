using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using inti2008.Data;

namespace inti2008.Web
{
    public partial class AdminEditRules : IntiPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess("ADMIN_TOURMASTER");

            if (!IsPostBack)
                LoadRules();
        }

        private void LoadRules()
        {
            using (var db = Global.GetConnection())
            {
                var rules = from r in db.Inti_TournamentRule
                            where r.TournamentGUID == SessionProps.SelectedTournament.GUID
                            select r;

                grdRules.DataSource = rules.OrderBy(r => r.SortOrder).ToList();
                grdRules.DataBind();

                lblSelectedTournament.Text = SessionProps.SelectedTournament.Name;

            }
        }

        private void CopyFromTournament(Guid tournamentId)
        {
            using (var db = Global.GetConnection())
            {
                var oldRules = from r in db.Inti_TournamentRule
                            where r.TournamentGUID == SessionProps.SelectedTournament.GUID
                            select r;

                if(oldRules.ToList().Count == 0)
                {
                    var rulesToCopy = from r in db.Inti_TournamentRule
                                      where r.TournamentGUID == tournamentId
                                      select r;

                    foreach (var rule in rulesToCopy)
                    {
                        var newRule = new Inti_TournamentRule();
                        newRule.Header = rule.Header;
                        newRule.TournamentGUID = SessionProps.SelectedTournament.GUID;
                        newRule.Body = rule.Body;
                        newRule.SortOrder = rule.SortOrder;

                        db.Inti_TournamentRule.InsertOnSubmit(newRule);

                    }

                    db.SubmitChanges();

                    //refresh the rules
                    LoadRules();
                }


            }
        }

        protected void grdRules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName == "New")
            {
                //get the highest sort order and add that to the list. Add some random rules and save and show rules
                using (var db = Global.GetConnection())
                {
                    var highestSortOrder =
                        db.Inti_TournamentRule.Where(r => r.TournamentGUID == SessionProps.SelectedTournament.GUID).Max(
                            r => r.SortOrder);

                    var newRule = new Inti_TournamentRule();
                    newRule.TournamentGUID = SessionProps.SelectedTournament.GUID;
                    newRule.SortOrder = highestSortOrder + 1;
                    newRule.Header = "header";
                    newRule.Body = "body";

                    db.Inti_TournamentRule.InsertOnSubmit(newRule);

                    db.SubmitChanges();
                }

                LoadRules();


            }
        }

        protected void grdRules_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdRules.EditIndex = e.NewEditIndex;
            LoadRules();
        }

        protected void grdRules_RowDeleting(object sender, GridViewDeleteEventArgs gridViewDeleteEventArgs)
        {
            grdRules.EditIndex = gridViewDeleteEventArgs.RowIndex;

            using (var db = Global.GetConnection())
            {
                //do the delete
                var ruleSortOrder = int.Parse(grdRules.Rows[grdRules.EditIndex].Cells[2].Text);

                var rule =
                    db.Inti_TournamentRule.Single(
                        r =>
                        r.TournamentGUID == SessionProps.SelectedTournament.GUID && r.SortOrder == ruleSortOrder);

                db.Inti_TournamentRule.DeleteOnSubmit(rule);
                

                db.SubmitChanges();
            }

            LoadRules();
        }

        protected void grdRules_CancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdRules.EditIndex = -1;
            LoadRules();
        }

        protected void grdRules_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if(grdRules.EditIndex >= 0)
            {
                //do the save
                var ruleSortOrder = int.Parse(grdRules.Rows[grdRules.EditIndex].Cells[2].Text);

                using(var db = Global.GetConnection())
                {
                    var rule =
                        db.Inti_TournamentRule.Single(
                            r =>
                            r.TournamentGUID == SessionProps.SelectedTournament.GUID && r.SortOrder == ruleSortOrder);

                    rule.Header = (grdRules.Rows[grdRules.EditIndex].Cells[0].Controls[0] as TextBox).Text;
                    rule.Body = (grdRules.Rows[grdRules.EditIndex].Cells[1].Controls[0] as TextBox).Text;

                    db.SubmitChanges();
                }
            }

            grdRules.EditIndex = -1;
            LoadRules();
        }

        protected void grdRules_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //var rule = new Inti_TournamentRule();

            //rule.SortOrder = int.Parse(e.Row.Cells[2].Text);
            //rule.TournamentGUID = SessionProps.SelectedTournament.GUID;
            //rule.Header = (e.Row.Cells[0].Controls[0] as TextBox).Text;
            //rule.Body = (e.Row.Cells[1].Controls[0] as TextBox).Text;

            //using (var db = Global.GetConnection())
            //{
            //    db.Inti_TournamentRule.InsertOnSubmit(rule);
               
            //    db.SubmitChanges();
            //}
        }
    }
}
