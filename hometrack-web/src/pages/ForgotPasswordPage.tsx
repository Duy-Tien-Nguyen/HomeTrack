import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './ForgotPasswordPage.css';

const ForgotPasswordPage: React.FC = () => {
    const [email, setEmail] = useState('');
    const [verificationCode, setVerificationCode] = useState('');
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        setMessage(null);
        setError(null);
        // Here you would typically send a request to your backend
        // to initiate the password reset process.
        console.log('Forgot password request for:', email);
        if (email) {
            setMessage('If an account with that email exists, a password reset link has been sent.');
        } else {
            setError('Please enter your email address.');
        }
    };

    const handleSendCode = () => {
        console.log('Send verification code to:', email);
        // Implement sending verification code logic here
        alert('Mã xác nhận đã được gửi đến email của bạn.');
    };

    return (
        <div className="forgot-password-page-container">
            <div className="forgot-password-form-container">
                <div className="key-icon">
                    <img src="/assets/images/key-icon.png" alt="Key Icon" /> {/* Placeholder for key icon */}
                </div>
                <h2>Quên mật khẩu?</h2>
                <p>Nhập email của bạn để nhận liên kết đặt lại mật khẩu</p>
                <form onSubmit={handleSubmit}>
                    <div className="input-group">
                        <label htmlFor="email">Email</label>
                        <div className="email-input-wrapper">
                            <input
                                type="email"
                                id="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="Nhập địa chỉ email của bạn"
                                required
                            />
                            <button type="button" className="send-code-button" onClick={handleSendCode}>Gửi</button>
                        </div>
                    </div>
                    <div className="input-group">
                        <label htmlFor="verificationCode">Mã xác nhận</label>
                        <input
                            type="text"
                            id="verificationCode"
                            value={verificationCode}
                            onChange={(e) => setVerificationCode(e.target.value)}
                            required
                        />
                    </div>
                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}
                    <button type="submit" className="reset-password-button">Gửi liên kết đặt lại</button>
                </form>
                <p className="back-to-login-link">
                    <Link to="/"><span className="arrow-left">&leftarrow;</span> Quay lại đăng nhập</Link>
                </p>
            </div>
            <div className="background-image-container"></div>
        </div>
    );
};

export default ForgotPasswordPage; 