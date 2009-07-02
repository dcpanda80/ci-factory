
package com.targetprocess.integration.feature;

import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
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
 *         &lt;element name="featureID" type="{http://www.w3.org/2001/XMLSchema}int"/>
 *         &lt;element name="comment" type="{http://targetprocess.com}CommentDTO" minOccurs="0"/>
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
    "featureID",
    "comment"
})
@XmlRootElement(name = "AddCommentToFeature")
public class AddCommentToFeature {

    protected int featureID;
    protected CommentDTO comment;

    /**
     * Gets the value of the featureID property.
     * 
     */
    public int getFeatureID() {
        return featureID;
    }

    /**
     * Sets the value of the featureID property.
     * 
     */
    public void setFeatureID(int value) {
        this.featureID = value;
    }

    /**
     * Gets the value of the comment property.
     * 
     * @return
     *     possible object is
     *     {@link CommentDTO }
     *     
     */
    public CommentDTO getComment() {
        return comment;
    }

    /**
     * Sets the value of the comment property.
     * 
     * @param value
     *     allowed object is
     *     {@link CommentDTO }
     *     
     */
    public void setComment(CommentDTO value) {
        this.comment = value;
    }

}
