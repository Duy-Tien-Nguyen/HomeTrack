import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './SystemSettingsPage.css';

const SystemSettingsPage: React.FC = () => {
    const [freeLimit, setFreeLimit] = useState('50');
    const [packageName, setPackageName] = useState('');
    const [packagePrice, setPackagePrice] = useState('0');
    const [packageDescription, setPackageDescription] = useState('');
    const [packageDuration, setPackageDuration] = useState('12');
    const [packageStatus, setPackageStatus] = useState('Hoạt động');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        console.log('Saving settings:', {
            freeLimit,
            packageName,
            packagePrice,
            packageDescription,
            packageDuration,
            packageStatus,
        });
        // Implement actual save logic here, e.g., send to API
        alert('System settings saved successfully!');
    };

    return (
        <div className="system-settings-container">
            <aside className="sidebar">
                <div className="logo">
                    <h1>Cấu hình hệ thống</h1>
                </div>
                <nav className="main-nav">
                    <ul>
                        <li><Link to="/user-management">Quản lý người dùng</Link></li>
                        <li><Link to="/system-settings">Cấu hình hệ thống</Link></li>
                        <li><Link to="/monitoring">Giám sát & thống kê</Link></li>
                        <li><Link to="/content-moderation">Quản lý nội dung</Link></li>
                        <li><Link to="/">Logout</Link></li>
                    </ul>
                </nav>
            </aside>
            <main className="main-content">
                <header className="main-header">
                    <h2>Cấu hình hệ thống</h2>
                    <div className="header-right">
                        <span>Admin</span>
                        <div className="user-avatar">A</div>
                    </div>
                </header>
                <section className="settings-form-section">
                    <h3>Giới hạn Item</h3>
                    <form onSubmit={handleSubmit} className="settings-form">
                        <div className="form-group">
                            <label htmlFor="freeLimit">Giới hạn gói miễn phí</label>
                            <input
                                type="number"
                                id="freeLimit"
                                name="freeLimit"
                                value={freeLimit}
                                onChange={(e) => setFreeLimit(e.target.value)}
                            />
                            <p className="hint-text">Số lượng item tối đa cho người dùng gói miễn phí</p>
                        </div>

                        <h3 className="section-title">Tạo Gói dịch vụ</h3>
                        <p className="section-description">Vui lòng điền đầy đủ thông tin để tạo gói mới</p>

                        <div className="form-group">
                            <label htmlFor="packageName">Tên gói *</label>
                            <div className="input-with-icon">
                                <input
                                    type="text"
                                    id="packageName"
                                    name="packageName"
                                    value={packageName}
                                    onChange={(e) => setPackageName(e.target.value)}
                                    placeholder="Nhập tên gói dịch vụ"
                                    required
                                />
                                <i className="fas fa-chevron-down dropdown-icon"></i>
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="packagePrice">Giá (VND) *</label>
                            <div className="price-input-wrapper">
                                <span className="currency-prefix">VND</span>
                                <input
                                    type="number"
                                    id="packagePrice"
                                    name="packagePrice"
                                    value={packagePrice}
                                    onChange={(e) => setPackagePrice(e.target.value)}
                                    required
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="packageDescription">Mô tả</label>
                            <textarea
                                id="packageDescription"
                                name="packageDescription"
                                value={packageDescription}
                                onChange={(e) => setPackageDescription(e.target.value)}
                                placeholder="Mô tả chi tiết về gói dịch vụ..."
                                rows={3}
                            ></textarea>
                        </div>

                        <div className="form-group-inline">
                            <div className="form-group">
                                <label htmlFor="packageDuration">Thời gian hiệu lực *</label>
                                <div className="duration-input-wrapper">
                                    <input
                                        type="number"
                                        id="packageDuration"
                                        name="packageDuration"
                                        value={packageDuration}
                                        onChange={(e) => setPackageDuration(e.target.value)}
                                        required
                                    />
                                    <span className="unit">tháng</span>
                                </div>
                            </div>
                            <div className="form-group">
                                <label htmlFor="packageStatus">Trạng thái *</label>
                                <div className="status-select-wrapper">
                                    <span className={`status-dot ${packageStatus === 'Hoạt động' ? 'active' : ''}`}></span>
                                    <select
                                        id="packageStatus"
                                        name="packageStatus"
                                        value={packageStatus}
                                        onChange={(e) => setPackageStatus(e.target.value)}
                                        required
                                    >
                                        <option>Hoạt động</option>
                                        <option>Không hoạt động</option>
                                    </select>
                                    <i className="fas fa-chevron-down dropdown-icon"></i>
                                </div>
                            </div>
                        </div>

                        <div className="form-actions">
                            <button type="button" className="cancel-button">Hủy</button>
                            <button type="submit" className="save-settings-button">Lưu thay đổi</button>
                        </div>
                    </form>
                </section>
            </main>
        </div>
    );
};

export default SystemSettingsPage; 