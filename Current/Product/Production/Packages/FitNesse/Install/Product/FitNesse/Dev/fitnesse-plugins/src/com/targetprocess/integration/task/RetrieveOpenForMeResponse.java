
package com.targetprocess.integration.task;

import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Java class for anonymous complex type.
 * 
 * <p>The following schema fragment specifies the expected content contained within this class.
 * 
 * <pre>
 * &lt;complexType>
 *   &lt;complexContent>
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType">
 *       &lt;sequence>
 *         &lt;element name="RetrieveOpenForMeResult" type="{http://targetprocess.com}ArrayOfTaskDTO" minOccurs="0"/>
 *       &lt;/sequence>
 *     &lt;/restriction>
 *   &lt;/complexContent>
 * &lt;/complexType>
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "", propOrder = {
    "retrieveOpenForMeResult"
})
@XmlRootElement(name = "RetrieveOpenForMeResponse")
public class RetrieveOpenForMeResponse {

    @XmlElement(name = "RetrieveOpenForMeResult")
    protected ArrayOfTaskDTO retrieveOpenForMeResult;

    /**
     * Gets the value of the retrieveOpenForMeResult property.
     * 
     * @return
     *     possible object is
     *     {@link ArrayOfTaskDTO }
     *     
     */
    public ArrayOfTaskDTO getRetrieveOpenForMeResult() {
        return retrieveOpenForMeResult;
    }

    /**
     * Sets the value of the retrieveOpenForMeResult property.
     * 
     * @param value
     *     allowed object is
     *     {@link ArrayOfTaskDTO }
     *     
     */
    public void setRetrieveOpenForMeResult(ArrayOfTaskDTO value) {
        this.retrieveOpenForMeResult = value;
    }

}
