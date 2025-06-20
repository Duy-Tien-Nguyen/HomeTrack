import React from 'react';
import { Link } from 'react-router-dom';
import './ContentModerationPage.css';

const ContentModerationPage: React.FC = () => {
    const flaggedItems = [
        { id: 12345, title: 'Hình ảnh này được gắn tag không chính xác. Đây không phải là "bàn làm việc" mà là "bàn ăn".', reason: 'Sai tag', reportedBy: 'user123@example.com', date: '15/06/2023', status: 'Pending Review' },
        { id: 12346, title: 'Hình ảnh này chứa nội dung không phù hợp với quy định của hệ thống.', reason: 'Nội dung không phù hợp', reportedBy: 'user456@example.com', date: '14/06/2023', status: 'Pending Review' },
    ];

    const handleApprove = (id: number) => {
        console.log('Approve item:', id);
        // Implement actual approve logic here
    };

    const handleReject = (id: number) => {
        console.log('Reject item:', id);
        // Implement actual reject logic here
    };

    const handleSkip = (id: number) => {
        console.log('Skip item:', id);
        // Implement actual skip logic here
    };

    return (
        <div className="content-moderation-container">
            <aside className="sidebar">
                <div className="logo">
                    <h1>HomeTrack Admin</h1>
                </div>
                <nav className="main-nav">
                    <ul>
                        <li><Link to="/dashboard">Dashboard</Link></li>
                        <li><Link to="/user-management">User Management</Link></li>
                        <li><Link to="/system-settings">System Settings</Link></li>
                        <li><Link to="/monitoring">Monitoring</Link></li>
                        <li><Link to="/content-moderation">Content Moderation</Link></li>
                        <li><Link to="/">Logout</Link></li>
                    </ul>
                </nav>
            </aside>
            <main className="main-content">
                <header className="main-header">
                    <h2>Content Moderation Page</h2>
                    <div className="user-info">
                        <span>Admin</span>
                        <div className="user-avatar">A</div>
                    </div>
                </header>
                <section className="flagged-items-list">
                    <div className="flagged-items-header">
                        <h3>Danh sách báo cáo</h3>
                        <div className="filter-dropdown">
                            <select>
                                <option>Tất cả báo cáo</option>
                                <option>Đang chờ xử lý</option>
                                <option>Đã duyệt</option>
                                <option>Đã từ chối</option>
                            </select>
                        </div>
                    </div>

                    <div className="items-grid">
                        {flaggedItems.map((item) => (
                            <div key={item.id} className="report-card">
                                <div className="report-card-image">
                                    <img src="/assets/images/placeholder.png" alt="placeholder" /> {/* Placeholder image */}
                                </div>
                                <div className="report-card-content">
                                    <h4>Báo cáo #{item.id}</h4>
                                    <p className="report-description">{item.title}</p>
                                    <p className="reported-by">Báo cáo bởi: {item.reportedBy}</p>
                                    {item.reason && <span className="reason-tag">{item.reason}</span>}
                                    <span className="report-date">{item.date}</span>
                                </div>
                                <div className="report-card-actions">
                                    {item.status === 'Pending Review' ? (
                                        <>
                                            <button className="action-button skip-button" onClick={() => handleSkip(item.id)}>Bỏ qua</button>
                                            <button className="action-button approve-button" onClick={() => handleApprove(item.id)}>Duyệt</button>
                                            <button className="action-button reject-button" onClick={() => handleReject(item.id)}>Xoá</button>
                                        </>
                                    ) : (
                                        <span className="status-resolved">{item.status}</span>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>

                    <div className="pagination">
                        <button className="pagination-button">&leftarrow;</button>
                        <button className="pagination-button active">1</button>
                        <button className="pagination-button">2</button>
                        <button className="pagination-button">&rightarrow;</button>
                    </div>
                </section>
            </main>
        </div>
    );
};

export default ContentModerationPage;

