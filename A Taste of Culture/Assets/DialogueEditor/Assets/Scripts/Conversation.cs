using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------
// Conversation C# class - User Facing
//--------------------------------------

namespace DialogueEditor
{
    public enum eParamStatus
    {
        OK = 0,
        NoParamFound = 1,
    }

    public class Conversation
    {
        public Conversation()
        {
            Parameters = new List<Parameter>();
        }

        /// <summary> The start of the conversation </summary>
        public SpeechNode Root;

        /// <summary>
        /// The ID of the node we should start the conversation at. -1 = invalid/ignored.<br/>
        /// Added for restoring a save at a position in the conversation.
        /// </summary>
        public int IDToStartAt = -1;
        public SpeechNode NodeToStartAt = null;

        /// <summary> The parameters of this conversation, and their values </summary>
        public List<Parameter> Parameters;

        /// <summary> The font used for the 'Continue' button. </summary>
        public TMPro.TMP_FontAsset ContinueFont;

        /// <summary> The font used for the 'End' button. </summary>
        public TMPro.TMP_FontAsset EndConversationFont;

        /// <summary>
        /// The text continue buttons in this conversation should have.
        /// </summary>
        public string ContinueText;

        /// <summary>
        /// /// The text the end button in this conversation should have.
        /// </summary>
        public string EndText;

        public UnityEngine.Events.UnityEvent EventsOnEnd;

        // ---

        public void SetInt(string paramName, int value, out eParamStatus status)
        {
            IntParameter param = GetParameter(paramName) as IntParameter;
            if (param != null)
            {
                param.IntValue = value;
                status = eParamStatus.OK;
            }
            else
            {
                status = eParamStatus.NoParamFound;
            }
        }

        public void SetBool(string paramName, bool value, out eParamStatus status)
        {
            BoolParameter param = GetParameter(paramName) as BoolParameter;
            if (param != null)
            {
                param.BoolValue = value;
                status = eParamStatus.OK;
            }
            else
            {
                status = eParamStatus.NoParamFound;
            }
        }

        public int GetInt(string paramName, out eParamStatus status)
        {
            IntParameter param = GetParameter(paramName) as IntParameter;
            if (param != null)
            {
                status = eParamStatus.OK;
                return param.IntValue;
            }
            else
            {
                status = eParamStatus.NoParamFound;
            }
            return 0;
        }

        public bool GetBool(string paramName, out eParamStatus status)
        {
            BoolParameter param = GetParameter(paramName) as BoolParameter;
            if (param != null)
            {
                status = eParamStatus.OK;
                return param.BoolValue;

            }
            else
            {
                status = eParamStatus.NoParamFound;
            }
            return false;
        }

        private Parameter GetParameter(string name)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].ParameterName == name)
                    return Parameters[i];
            }
            return null;
        }
    }
}