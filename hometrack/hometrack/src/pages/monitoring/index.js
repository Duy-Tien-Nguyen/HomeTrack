import classNames from "classnames/bind";
import style from "./index.module.scss";

import { logSystem, listMonit } from "./listMonitoring";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSortDown } from "@fortawesome/free-solid-svg-icons";

const cx = classNames.bind(style);

function Monitoring() {
  const listM = listMonit;
  const listL = logSystem;
  return (
    <div className={cx("wrapper")}>
      <div className={cx("header")}>
        <div className={cx("header-content")}>
          <div className={cx("header-content-notification")}>Báo cáo</div>
          <div className={cx("header-content-date")}>
            <div className={cx('header-content-date-ct')}>Tháng 6,2023</div>
            <FontAwesomeIcon className={cx('header-content-date-icon')} icon={faSortDown} />
            </div>
        </div>
        <div className={cx("header-parameter")}>
          <div className={cx("header-parameter-list")}>
            {listM.map((item, index) => {
              return (
                <div className={cx("list-item")} key={index}>
                  <div className={cx("header-content-user")}>{item.title}</div>
                  <div className={cx("header-parameter-user")}>
                    {item.number}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>
      <div className={cx("main")}>
        <div className={cx("main-content")}>Thống kê người dùng</div>
        <div className={cx("main-parameter")}>
          <div className={cx("main-content-free")}>
            <div className={cx("main-content-free-ct")}>Miễn phí</div>
            <div className={cx("main-content-free-para")}>
              <div className={cx("color-free")}></div>
            </div>
            <div className={cx("main-content-free-res")}>1,245</div>
          </div>
          <div className={cx("main-content-pay")}>
            <div className={cx("main-content-pay-ct")}>trả phí</div>

            <div className={cx("main-content-pay-para")}>
              <div className={cx("color-pay")}></div>
            </div>
            <div className={cx("main-content-pay-res")}>458</div>
          </div>
        </div>
        <div className={cx("result-user")}>
          <div className={cx("result-user-content")}>tổng người dùng</div>
          <div className={cx("result-user-number")}>1,830</div>
        </div>
      </div>
      <div className={cx("footer")}>
        <div className={cx("footer-content")}>
          <div className={cx("footer-content-ct")}>Logs hệ thống</div>
          <div className={cx("footer-content-short")}>
            <FontAwesomeIcon icon={faSortDown} className={cx("icon-short")} />
           <div className={cx('footer-content-short-ct')}> Lọc</div>
          </div>
        </div>
        <div className={cx("    ")}></div>
        <div className={cx("footer-list-all")}>
          {listL.map((item, index) => {
            return (
              <div className={cx("footer-list")} key={index}>
                <div className={cx("list-time")}>{item.time}</div>
                <div
                  className={cx("list-info")}
                  style={{
                    backgroundColor: item.backrought,
                    color: item.color,
                  }}
                >
                  {item.info}
                </div>
                <div className={cx("list-content")}>{item.data}</div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

export default Monitoring;
