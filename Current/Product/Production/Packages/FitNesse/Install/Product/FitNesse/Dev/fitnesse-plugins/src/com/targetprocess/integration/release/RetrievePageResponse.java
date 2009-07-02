
package com.targetprocess.integration.release;

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
 *         &lt;element name="RetrievePageResult" type="{http://targetprocess.com}ArrayOfReleaseDTO" minOccurs="0"/>
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
    "retrievePageResult"
})
@XmlRootElement(name = "RetrievePageResponse")
public class RetrievePageResponse {

    @XmlElement(name = "RetrievePageResult")
    protected ArrayOfReleaseDTO retrievePageResult;

    /**
     * Gets the value of the retrievePageResult property.
     * 
     * @return
     *     possible object is
     *     {@link ArrayOfReleaseDTO }
     *     
     */
    public ArrayOfReleaseDTO getRetrievePageResult() {
        return retrievePageResult;
    }

    /**
     * Sets the value of the retrievePageResult property.
     * 
     * @param value
     *     allowed object is
     *     {@link ArrayOfReleaseDTO }
     *     
     */
    public void setRetrievePageResult(ArrayOfReleaseDTO value) {
        this.retrievePageResult = value;
    }

}
