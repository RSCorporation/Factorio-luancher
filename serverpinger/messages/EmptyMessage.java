/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package by.radioegor146.serverpinger.messages;

import by.radioegor146.serverpinger.ClientMessage;
import by.radioegor146.serverpinger.classes.ModInfo;
import by.radioegor146.serverpinger.classes.ModSettings;
import by.radioegor146.serverpinger.utils.DataStreamUtils;
import by.radioegor146.serverpinger.utils.LittleEndianOutputStream;
import java.io.ByteArrayOutputStream;

/**
 *
 * @author radioegor146
 */
public class EmptyMessage extends ClientMessage {

    @Override
    public byte[] getBytes() {
        return new byte[0];
    }
    
    @Override
    public byte getMessageType() {
        return 0x12;
    }
}
