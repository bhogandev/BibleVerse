﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories.UserRespositories
{
    public class UserRepositoriesHelper
    {

        private static string StackTrace = "BibleVerse.Repositories -> UserRepositories -> UserRepositoriesHelper";

        #region Public Methods

        #region User Relationships
        //For Sending Relationships (Initiation)
        public static bool ProcessUserRelationshipSend(BibleVerse.DTO.UserRelationships _relationship, BibleVerse.DTO.RelationshipRequestModel _request)
        {
            try
            {
                string entType = _relationship.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(_relationship);

                bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                bool userHistoryResult = LogActionInUserHistory("UserRequest", ReturnActionMessageForSend(_request.RequestType, _request));

                return userHistoryResult;

            } catch(Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }

        //To cancel prior relationship request
        public static bool ProcessUserRelationshipCancel(BibleVerse.DTO.UserRelationships _relationship, BibleVerse.DTO.RelationshipRequestModel _request, BibleVerse.DTO.Notifications _removableNotification)
        {
            try
            {
                string entType = _relationship.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(_relationship);

                bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                bool userHistoryResult = LogActionInUserHistory("UserRequest", ReturnActionMessageForCancel(_request.RequestType, _request, _removableNotification, _relationship));

                return userHistoryResult;

            } catch(Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }

        //To accept prior relationship request
        public static bool ProcessUserRelationshipAccept(BibleVerse.DTO.UserRelationships _relationship, BibleVerse.DTO.RelationshipRequestModel _request, BibleVerse.DTO.Users _firstUser, BibleVerse.DTO.Users _secondUser)
        {
            try
            {
                string entType = _relationship.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(_relationship);

                bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                bool userHistoryResult = LogActionInUserHistory("UserRequest", ReturnActionMessageForAccept(_request.RequestType, _request, _relationship, _firstUser, _secondUser));

                return userHistoryResult;

            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }

        #endregion

        #region User Queries



        #endregion

        #endregion

        #region Private Methods

        private static string ReturnActionMessageForSend(string _requestType, BibleVerse.DTO.RelationshipRequestModel _request)
        {
            try
            {
                if (!String.IsNullOrEmpty(_requestType) && _request != null)
                {
                    switch (_requestType)
                    {
                        case "Send friend request":
                            //Create user notification for friend request
                            BibleVerse.Events.NotificationEvent notificationEvent = new Events.NotificationEvent();
                            notificationEvent.CreateNotification(_request.SecondUser, _request.FirstUser, _request.FirstUser + " has sent you a friend request.", "Friend Request", "");
                            return String.Format("{0} sent {1} a friend request.", _request.FirstUser, _request.SecondUser);

                        /* case "Cancel friend request":
                             BibleVerse.
                             return String.Format("{0} canceled a friend request to {1}.", _request.FirstUser, _request.SecondUser);
                        */

                        default:
                            return String.Empty;
                    }
                }else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }catch(Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return String.Empty;
            }
        }

        private static string ReturnActionMessageForCancel(string _requestType, BibleVerse.DTO.RelationshipRequestModel _request, BibleVerse.DTO.Notifications _removableNotifiation, BibleVerse.DTO.UserRelationships _removeRelationship)
        {
            try
            {
                if (!String.IsNullOrEmpty(_requestType) && _request != null && _removableNotifiation != null && _removeRelationship != null)
                {
                    switch (_requestType)
                    {
                        case "Cancel friend request":
                            
                            //Remove Relationship
                            string entType = _removeRelationship.GetType().FullName;
                            string entObj = JsonConvert.SerializeObject(_removeRelationship);
                            bool result = BVCommon.BVContextFunctions.DeleteFromDb(entType, entObj);

                            if (result)
                            {
                                //Remove Notification
                                entType = _removableNotifiation.GetType().FullName;
                                entObj = JsonConvert.SerializeObject(_removableNotifiation);
                                result = BVCommon.BVContextFunctions.DeleteFromDb(entType, entObj);
                                return result == true ? String.Format("{0} canceled a friend request to {1}.", _request.FirstUser, _request.SecondUser) : "";
                            }
                            else
                            {
                                return String.Empty;
                            }

                        default:
                            return String.Empty;
                    }
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return String.Empty;
            }
        }

        private static string ReturnActionMessageForAccept(string _requestType, BibleVerse.DTO.RelationshipRequestModel _request, BibleVerse.DTO.UserRelationships _relationship, BibleVerse.DTO.Users _firstUser, BibleVerse.DTO.Users _secondUser)
        {
            try
            {
                if (!String.IsNullOrEmpty(_requestType) && _request != null && _relationship != null && _firstUser != null && _secondUser != null)
                {
                    switch (_requestType)
                    {
                        case "Accept friend request":
                            _relationship.SecondUserConfirmed = true; // Confirm friend request for second user
                            string entType = _relationship.GetType().FullName;
                            string entObj = JsonConvert.SerializeObject(_relationship);
                            bool relationshipWasUpdated = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);

                            if (relationshipWasUpdated)
                            {
                                //Create Notification for Accepting
                                BibleVerse.Events.NotificationEvent notificationEvent = new Events.NotificationEvent();
                                notificationEvent.CreateNotification(_request.SecondUser, _request.FirstUser, _request.FirstUser + " has accepted your friend request.", "Friend Request", "");

                                //Increment users Friend counts
                                _firstUser.Friends++;
                                _secondUser.Friends++;

                                entType = _firstUser.GetType().FullName;
                                entObj = JsonConvert.SerializeObject(_firstUser);
                                bool FirstUserWasUpdated = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);

                                entType = _secondUser.GetType().FullName;
                                entObj = JsonConvert.SerializeObject(_secondUser);
                                bool SecondUserWasUpdated = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);

                                if(FirstUserWasUpdated && SecondUserWasUpdated)
                                {
                                    BibleVerse.DTO.UserHistory actionLogRelation = new DTO.UserHistory()
                                    {
                                        ActionType = "RelationshipUpdate",
                                        ActionMessage = _request.FirstUser + " & " + _request.SecondUser + " are now friends",
                                        ChangeDateTime = DateTime.Now,
                                        CreateDateTime = DateTime.Now
                                    };

                                    entType = actionLogRelation.GetType().FullName;
                                    entObj = JsonConvert.SerializeObject(actionLogRelation);
                                    bool actionLogWasUpdated = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);

                                    if(!actionLogWasUpdated)
                                    {
                                        throw new Exception()
                                        {
                                            Source = "User History Was Not Updated For Accept Request"
                                        };
                                    }
                                }else
                                {
                                    throw new Exception()
                                    {
                                        Source = "One or both User Friend Counts were not Updated"
                                    };
                                }
                            } else
                            {
                                throw new Exception()
                                {
                                    Source = "Relationship was not updated in db"
                                };
                            }
                            return _request.SecondUser + " accepted a friend request from " + _request.FirstUser;

                        default:
                            return String.Empty;
                    }
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return String.Empty;
            }
        }

        //For All Other Requests
        private static bool LogActionInUserHistory(string _actionType, string _actionMessage)
        {
            try
            {
                if (!String.IsNullOrEmpty(_actionType) && !String.IsNullOrEmpty(_actionMessage))
                {
                    BibleVerse.DTO.UserHistory userHistoryEntry = new DTO.UserHistory()
                    {
                        ActionType = _actionType,
                        ActionMessage = _actionMessage,
                        CreateDateTime = DateTime.Now,
                        ChangeDateTime = DateTime.Now
                    };

                    string entType = userHistoryEntry.GetType().FullName;

                    string entObj = JsonConvert.SerializeObject(userHistoryEntry);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                    return result;
                } else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            } catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }

        #endregion
    }
}
