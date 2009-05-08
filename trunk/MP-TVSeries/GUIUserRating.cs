using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using System.ComponentModel;

namespace WindowPlugins.GUITVSeries {
    public class GUIUserRating : GUIDialogWindow {        
        public const int ID = 9814;

        public GUIUserRating() {
            GetID = ID;
        }

        [SkinControlAttribute(6)]
        protected GUILabelControl lblText = null;
		[SkinControlAttribute(7)]
		protected GUILabelControl lblRating = null;
        [SkinControlAttribute(100)]
        protected GUIToggleButtonControl btnStar1 = null;
        [SkinControlAttribute(101)]
        protected GUIToggleButtonControl btnStar2 = null;
        [SkinControlAttribute(102)]
        protected GUIToggleButtonControl btnStar3 = null;
        [SkinControlAttribute(103)]
        protected GUIToggleButtonControl btnStar4 = null;
        [SkinControlAttribute(104)]
        protected GUIToggleButtonControl btnStar5 = null;
        [SkinControlAttribute(105)]
        protected GUIToggleButtonControl btnStar6 = null;
        [SkinControlAttribute(106)]
        protected GUIToggleButtonControl btnStar7 = null;
        [SkinControlAttribute(107)]
        protected GUIToggleButtonControl btnStar8 = null;
        [SkinControlAttribute(108)]
        protected GUIToggleButtonControl btnStar9 = null;
        [SkinControlAttribute(109)]
        protected GUIToggleButtonControl btnStar10 = null;

        public string Text {
            get {
                return lblText.Label;
            }

            set {
                lblText.Label = value;				
            }
        }		

        public int Rating { get; set; }
        public bool IsSubmitted { get; set; }

        public override void Reset() {
            base.Reset();

            SetHeading("");
            SetLine(1, "");
            SetLine(2, "");
            SetLine(3, "");
            SetLine(4, "");
        }

		public override void DoModal(int ParentID)
		{
			LoadSkin();
			AllocResources();
			InitControls();

			base.DoModal(ParentID);
		}

        public override bool Init() {
            return Load(GUIGraphicsContext.Skin + @"\TVSeries.RatingDialog.xml");
        }

        public override void OnAction(Action action) {
            switch (action.wID) {
                case Action.ActionType.REMOTE_1:
                    Rating = 1;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_2:
                    Rating = 2;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_3:
                    Rating = 3;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_4:
                    Rating = 4;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_5:
                    Rating = 5;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_6:
                    Rating = 6;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_7:
                    Rating = 7;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_8:
                    Rating = 8;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_9:
                    Rating = 9;
                    UpdateRating();
                    break;
                case Action.ActionType.REMOTE_0:
                    Rating = 10;
                    UpdateRating();
                    break;
                case Action.ActionType.ACTION_SELECT_ITEM:
                    IsSubmitted = true;
                    PageDestroy();
                    return;
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                case Action.ActionType.ACTION_CLOSE_DIALOG:
                case Action.ActionType.ACTION_CONTEXT_MENU:
                    IsSubmitted = false;
                    PageDestroy();
                    return;
            }

            base.OnAction(action);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            base.OnClicked(controlId, control, actionType);
            if (control == btnStar1) {
                Rating = 1;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar2) {
                Rating = 2;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar3) {
                Rating = 3;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar4) {
                Rating = 4;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar5) {
                Rating = 5;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar6) {
                Rating = 6;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar7) {
                Rating = 7;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar8) {
                Rating = 8;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar9) {
                Rating = 9;
                IsSubmitted = true;
                PageDestroy();
                return;
            }
            else if (control == btnStar10) {
                Rating = 10;
                IsSubmitted = true;
                PageDestroy();
                return;
            }

        }

        public override bool OnMessage(GUIMessage message) {
            switch (message.Message) {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    base.OnMessage(message);
                    IsSubmitted = false;
                    UpdateRating();
                    return true;

                case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
                    if (message.TargetControlId < 100 || message.TargetControlId > 109)
                        break;

                    Rating = message.TargetControlId - 99;
                    UpdateRating();
                    break;
            }
            return base.OnMessage(message);
        }

        private void UpdateRating() {
            GUIToggleButtonControl[] btnStars = new GUIToggleButtonControl[10] { btnStar1, btnStar2, btnStar3, btnStar4, btnStar5,
                                                                                 btnStar6, btnStar7, btnStar8, btnStar9, btnStar10 };
            for (int i = 0; i < 10; i++) {
                btnStars[i].Selected = (Rating >= i + 1);
            }
            btnStars[Rating - 1].Focus = true;

			if (lblRating != null) {
				lblRating.Label = string.Format("{0} / 10", Rating.ToString());
			}
        }

        public void SetHeading(string HeadingLine) {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, 1, 0, 0, null);
            msg.Label = HeadingLine;
            OnMessage(msg);
        }

        public void SetLine(int LineNr, string Line) {
            if (LineNr < 1) return;
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, 1 + LineNr, 0, 0, null);
            msg.Label = Line;
            if ((msg.Label == string.Empty) || (msg.Label == "")) msg.Label = "  ";
            OnMessage(msg);
        }
    }
}
