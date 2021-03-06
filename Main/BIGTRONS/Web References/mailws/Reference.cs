﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace com.SML.BIGTRONS.mailws {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="MailSenderSoap", Namespace="http://tempuri.org/")]
    public partial class MailSender : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback SendMailSimpleOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendMailOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendMailDeliverySimpleOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendMailDeliveryOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public MailSender() {
            this.Url = global::com.SML.BIGTRONS.Properties.Settings.Default.com_SML_BIGTRONS_mailws_MailSender;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event SendMailSimpleCompletedEventHandler SendMailSimpleCompleted;
        
        /// <remarks/>
        public event SendMailCompletedEventHandler SendMailCompleted;
        
        /// <remarks/>
        public event SendMailDeliverySimpleCompletedEventHandler SendMailDeliverySimpleCompleted;
        
        /// <remarks/>
        public event SendMailDeliveryCompletedEventHandler SendMailDeliveryCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMailSimple", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SendMailSimple(string From, string To, string CC, string Bcc, string Subject, string Body) {
            object[] results = this.Invoke("SendMailSimple", new object[] {
                        From,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SendMailSimpleAsync(string From, string To, string CC, string Bcc, string Subject, string Body) {
            this.SendMailSimpleAsync(From, To, CC, Bcc, Subject, Body, null);
        }
        
        /// <remarks/>
        public void SendMailSimpleAsync(string From, string To, string CC, string Bcc, string Subject, string Body, object userState) {
            if ((this.SendMailSimpleOperationCompleted == null)) {
                this.SendMailSimpleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailSimpleOperationCompleted);
            }
            this.InvokeAsync("SendMailSimple", new object[] {
                        From,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body}, this.SendMailSimpleOperationCompleted, userState);
        }
        
        private void OnSendMailSimpleOperationCompleted(object arg) {
            if ((this.SendMailSimpleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailSimpleCompleted(this, new SendMailSimpleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMail", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SendMail(string From, string To, string CC, string Bcc, string Subject, string Body, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)] MailAttachment[] Attachments) {
            object[] results = this.Invoke("SendMail", new object[] {
                        From,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body,
                        Attachments});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SendMailAsync(string From, string To, string CC, string Bcc, string Subject, string Body, MailAttachment[] Attachments) {
            this.SendMailAsync(From, To, CC, Bcc, Subject, Body, Attachments, null);
        }
        
        /// <remarks/>
        public void SendMailAsync(string From, string To, string CC, string Bcc, string Subject, string Body, MailAttachment[] Attachments, object userState) {
            if ((this.SendMailOperationCompleted == null)) {
                this.SendMailOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailOperationCompleted);
            }
            this.InvokeAsync("SendMail", new object[] {
                        From,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body,
                        Attachments}, this.SendMailOperationCompleted, userState);
        }
        
        private void OnSendMailOperationCompleted(object arg) {
            if ((this.SendMailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailCompleted(this, new SendMailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMailDeliverySimple", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SendMailDeliverySimple(string From, string To, string CC, string Bcc, string Subject, string Body, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)] MailAttachment[] Attachments, bool DeliveryNotification) {
            object[] results = this.Invoke("SendMailDeliverySimple", new object[] {
                        From,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body,
                        Attachments,
                        DeliveryNotification});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SendMailDeliverySimpleAsync(string From, string To, string CC, string Bcc, string Subject, string Body, MailAttachment[] Attachments, bool DeliveryNotification) {
            this.SendMailDeliverySimpleAsync(From, To, CC, Bcc, Subject, Body, Attachments, DeliveryNotification, null);
        }
        
        /// <remarks/>
        public void SendMailDeliverySimpleAsync(string From, string To, string CC, string Bcc, string Subject, string Body, MailAttachment[] Attachments, bool DeliveryNotification, object userState) {
            if ((this.SendMailDeliverySimpleOperationCompleted == null)) {
                this.SendMailDeliverySimpleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailDeliverySimpleOperationCompleted);
            }
            this.InvokeAsync("SendMailDeliverySimple", new object[] {
                        From,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body,
                        Attachments,
                        DeliveryNotification}, this.SendMailDeliverySimpleOperationCompleted, userState);
        }
        
        private void OnSendMailDeliverySimpleOperationCompleted(object arg) {
            if ((this.SendMailDeliverySimpleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailDeliverySimpleCompleted(this, new SendMailDeliverySimpleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMailDelivery", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SendMailDelivery(string From, string ReplyTo, string NotificationTo, string To, string CC, string Bcc, string Subject, string Body, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)] MailAttachment[] Attachments, bool DeliveryNotification) {
            object[] results = this.Invoke("SendMailDelivery", new object[] {
                        From,
                        ReplyTo,
                        NotificationTo,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body,
                        Attachments,
                        DeliveryNotification});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SendMailDeliveryAsync(string From, string ReplyTo, string NotificationTo, string To, string CC, string Bcc, string Subject, string Body, MailAttachment[] Attachments, bool DeliveryNotification) {
            this.SendMailDeliveryAsync(From, ReplyTo, NotificationTo, To, CC, Bcc, Subject, Body, Attachments, DeliveryNotification, null);
        }
        
        /// <remarks/>
        public void SendMailDeliveryAsync(string From, string ReplyTo, string NotificationTo, string To, string CC, string Bcc, string Subject, string Body, MailAttachment[] Attachments, bool DeliveryNotification, object userState) {
            if ((this.SendMailDeliveryOperationCompleted == null)) {
                this.SendMailDeliveryOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailDeliveryOperationCompleted);
            }
            this.InvokeAsync("SendMailDelivery", new object[] {
                        From,
                        ReplyTo,
                        NotificationTo,
                        To,
                        CC,
                        Bcc,
                        Subject,
                        Body,
                        Attachments,
                        DeliveryNotification}, this.SendMailDeliveryOperationCompleted, userState);
        }
        
        private void OnSendMailDeliveryOperationCompleted(object arg) {
            if ((this.SendMailDeliveryCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailDeliveryCompleted(this, new SendMailDeliveryCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1590.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class MailAttachment {
        
        private string fileNameField;
        
        private byte[] contentField;
        
        /// <remarks/>
        public string FileName {
            get {
                return this.fileNameField;
            }
            set {
                this.fileNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute()]
        public byte[] Content {
            get {
                return this.contentField;
            }
            set {
                this.contentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void SendMailSimpleCompletedEventHandler(object sender, SendMailSimpleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendMailSimpleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendMailSimpleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void SendMailCompletedEventHandler(object sender, SendMailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendMailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendMailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void SendMailDeliverySimpleCompletedEventHandler(object sender, SendMailDeliverySimpleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendMailDeliverySimpleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendMailDeliverySimpleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void SendMailDeliveryCompletedEventHandler(object sender, SendMailDeliveryCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendMailDeliveryCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendMailDeliveryCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591