using System;
using UnityEngine;


public class PublicLabFactors : MonoBehaviour
{
    public const int block_start_index = 1;
    public const int trial_start_index = 1;

    // WelcomePhase for server
    public enum WelcomePhase
    {
        in_entry_scene = 0,
        wait_for_input_information = 1,
        check_client_scene = 2,
        set_target_lab = 3,
        assign_block_conditions = 4,
        accept_acc_from_now = 5,
        adjust_block_conditions = 6,
        confirm_block_conditions = 7,
        ready_to_enter_lab = 8,
        in_lab_scene = 9,
    }


    public enum LabScene
    {
        Entry_scene = 0,
        Lab0_tap_5_5 = 1,
        Lab1_tap_33_33 = 2,
        Lab2_tap_57_57 = 3,
    }

    public enum LabMode
    {
        Full = 0,
        Test = 1,
    }

    public enum MessageType
    {
        Command = 0,
        Block = 1,
        Angle = 2,
        Trial = 3,
        Scene = 4,
    }

    public enum ServerCommand
    {
        no_server_command = 0,
        server_set_target_lab = 1,
        server_begin_to_receive_acc = 2,
        server_confirm_block_conditions = 3,
        server_say_enter_lab = 4,
        server_say_exit_lab = 5,
        server_say_end_lab = 6,
    }

    public enum ClientCommand
    {
        no_client_command = 0,
        client_finish_lab_set = 1,
        client_begin_to_deliver_acc = 2,
        client_confirm_block_conditions = 3,
        client_enter_lab = 4,
    }

    #region Trial conditions
    enum Factors
    {
        Posture = 0,
        Orientation = 1,
        Shape = 2,
        AngleBetweenScreens = 3,
        TargetSize = 4
    }

    public class Lab0_tap_55
    {
        public const int
            totalBlockCount = 48,
            fullTrialCount = 5 * 5,
            testTrialCount = 5 * 5,
            repetitionCount = 1,
            s1PositionCount = 5,
            s2PositionCount = 5;

        public enum Posture
        {       
            left1_right1 = 0,
            left1_right2 = 1,
            right1_right2 = 2,
            right2_right3 = 3,
        }
        public enum Orientation
        {
            protrait = 0,
            landscape = 1
        }

        public enum Shape
        {
            concave = 0,
            convex = 1
        }

        public static float[] AngleBetweenScreens = { 60, 90, 135, 225, 270, 300 };
    }

    public class Lab1_tap_99
    {
        public const int
            totalBlockCount = 60,
            fullTrialCount = 9 * 9,
            testTrialCount = 5,
            repetitionCount = 1,
            s1PositionCount = 9,
            s2PositionCount = 9,
            s1ColumnCount = 3, s1RowCount = 3,
            s2ColumnCount = 3, s2RowCount = 3;
        public enum Posture
        {
            none_right1 = 0,           // old_rightForefinger = 0,
            left1_right1 = 1,           // new_both_thumbs = 1,
            left1_right2 = 2,           // new_leftThumb_rightForefinger = 2,
            right1_right2 = 3,           // new_rightThumb_rightForefinger = 3,
            right2_right3 = 4,           // new_rightForefinger_rightMiddlefinger = 4,
        }
        public enum Orientation
        {
            protrait = 0,
            landscape = 1
        }

        public enum Shape
        {
            concave = 0,
            convex = 1
        }

        public static float[] AngleBetweenScreens = { 60, 90, 135, 225, 270, 300 };
        public static float[] TargetSize = { 1 };
    }

    public class Lab2_tap_3535
    {
        public const int
            totalBlockCount = 64,
            totalTrialCount = 35 * 35,
            repetitionCount = 3,
            s1PositionCount = 35,
            s2PositionCount = 35,
            s1ColumnCount = 5, s1RowCount = 7,
            s2ColumnCount = 5, s2RowCount = 7;
    }
    #endregion

    public struct LabInfos
    {
        public LabScene labName;
        public LabMode labMode;
        public int totalBlockCount, totalTrialCount, repetitionCount,
        s1PositionCount, s2PositionCount,
            s1ColumnCount, s1RowCount,
            s2ColumnCount, s2RowCount;

        public int postureCount, angleCount, orientationCount;

        public void setLabInfoWithName(LabScene name)
        {
            labName = name;
            switch (name)
            {
                case LabScene.Lab0_tap_5_5:
                    setParams(Lab0_tap_55.totalBlockCount, Lab0_tap_55.fullTrialCount,
                        Lab0_tap_55.repetitionCount, Lab0_tap_55.s1PositionCount, Lab0_tap_55.s2PositionCount,
                        Enum.GetNames(typeof(Lab0_tap_55.Posture)).Length, Lab0_tap_55.AngleBetweenScreens.Length, Enum.GetNames(typeof(Lab0_tap_55.Orientation)).Length);
                    break;
                case LabScene.Lab1_tap_33_33:
                    setParams(Lab1_tap_99.totalBlockCount, Lab1_tap_99.fullTrialCount,
                        Lab1_tap_99.repetitionCount, Lab1_tap_99.s1PositionCount, Lab1_tap_99.s2PositionCount,
                        Lab1_tap_99.s1ColumnCount, Lab1_tap_99.s1RowCount,
                        Lab1_tap_99.s2ColumnCount, Lab1_tap_99.s2RowCount,
                        Enum.GetNames(typeof(Lab1_tap_99.Posture)).Length, Lab1_tap_99.AngleBetweenScreens.Length, Enum.GetNames(typeof(Lab1_tap_99.Orientation)).Length);
                    break;
                case LabScene.Lab2_tap_57_57:
                    setParams(Lab2_tap_3535.totalBlockCount, Lab2_tap_3535.totalTrialCount,
                        Lab2_tap_3535.s1ColumnCount, Lab2_tap_3535.s1RowCount,
                        Lab2_tap_3535.s2ColumnCount, Lab2_tap_3535.s2RowCount,
                        Lab2_tap_3535.repetitionCount, Lab2_tap_3535.s1PositionCount, Lab2_tap_3535.s2PositionCount,
                        5, 6, 2);
                    break;
                default:
                    break;
            }
        }

        private void setParams(int cntBlock, int cntTrial, int cntRepetition, int cntS1Pos, int cntS2Pos,
            int cntPosture, int cntAngle, int cntOrientation)
        {
            totalBlockCount = cntBlock;
            totalTrialCount = cntTrial;
            labMode = LabMode.Full;
            repetitionCount = cntRepetition;
            s1PositionCount = cntS1Pos;
            s2PositionCount = cntS2Pos;

            postureCount = cntPosture;
            angleCount = cntAngle;
            orientationCount = cntOrientation;
        }

        private void setParams(int cntBlock, int cntTrial, int cntRepetition, int cntS1Pos, int cntS2Pos,
            int cntS1Column, int cntS1Row, int cntS2Column, int cntS2Row,
            int cntPosture, int cntAngle, int cntOrientation)
        {
            totalBlockCount = cntBlock;
            totalTrialCount = cntTrial;
            labMode = LabMode.Full;
            repetitionCount = cntRepetition;
            s1PositionCount = cntS1Pos;
            s2PositionCount = cntS2Pos;

            postureCount = cntPosture;
            angleCount = cntAngle;
            orientationCount = cntOrientation;
        }

        public void setTotalTrialCount(bool isFullMode)
        {
            switch (labName) {
                case LabScene.Lab0_tap_5_5:
                    labMode = isFullMode ? LabMode.Full : LabMode.Test;
                    totalTrialCount = isFullMode ? Lab0_tap_55.fullTrialCount : Lab0_tap_55.testTrialCount;
                    break;
                case LabScene.Lab1_tap_33_33:
                    labMode = isFullMode ? LabMode.Full : LabMode.Test;
                    totalTrialCount = isFullMode ? Lab1_tap_99.fullTrialCount : Lab1_tap_99.testTrialCount;
                    break;
            }
        }
        
    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}