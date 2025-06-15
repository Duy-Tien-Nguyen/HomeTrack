import classNames from "classnames/bind";
import style from "./index.module.scss";

import { ListHome } from "./listHome";

const cx = classNames.bind(style);
function Home() {
  const list = ListHome;
  return (
    <div className={cx("wrapper")}>
      <div className={cx("inform")}>
        <div className={cx("result-user", "box")}>
          <div className={cx("content-result-user", "content")}>
            Tổng người dùng
          </div>
          <div className={cx("data-result-user", "data")}>2,543</div>
        </div>
        <div className={cx("revenue", "box")}>
          <div className={cx("content-revenue", "content")}>Doanh thu</div>
          <div className={cx("data-revenue", "data")}>đ 45.2M</div>
        </div>
        <div className={cx("order", "box")}>
          <div className={cx("content-order", "content")}>Đơn hàng</div>
          <div className={cx("data-order", "data")}>1,247</div>
        </div>
        <div className={cx("growth", "box")}>
          <div className={cx("content-growth", "content")}>Tăng trưởng</div>
          <div className={cx("data-growth", "data")}>+12.5%</div>
        </div>
      </div>
      <div className={cx("chart")}>
        <div className={cx("chart-revenue", "box")}>
          <div className={cx("chart-content")}>Biểu đồ doanh thu</div>
          <div className={cx("chart-item")}>Biểu đồ sẽ được hiển thị ở đây</div>
        </div>
        <div className={cx("recent-activity", "box")}>
          <div className={cx("content-recent")}>Hoạt động gần đây </div>
          <div className={cx('list')}>
              {list.map((item, index) => {
                return (
                  <div className={cx("recent")} key={index}>
                    <div className={cx("use-act")}>{item.title}</div>
                    <div className={cx("time-act")}>{item.time} phút trước</div>
                  </div>
                );
              })}
          </div>
        </div>
      </div>
    </div>
  );
}

export default Home;
