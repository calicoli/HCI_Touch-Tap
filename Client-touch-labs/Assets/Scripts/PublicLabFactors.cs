using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PublicLabFactors : MonoBehaviour
{
    public const int block_start_index = 1;
    public const int trial_start_index = 1;

    // WelcomePhase for client
    public enum WelcomePhase
    {
        in_entry_scene = 0,
        wait_for_input_serverip = 1,
        detect_connect_status = 2,
        wait_for_server_set_lab = 3,
        check_server_scene = 4,
        wait_for_server_accept_acc = 5,
        deliver_angle_info = 6,
        ready_to_enter_lab = 7,
        out_entry_scene = 8
    }

    public enum LabScene
    {
        Entry_scene = 0,
        Lab0_tap_5_5 = 1,
        Lab1_tap_33_33 = 2,
        Lab2_tap_57_57 = 3,
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
            testTrialCount = 5,
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

}