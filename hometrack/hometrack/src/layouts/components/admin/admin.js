import classNames from "classnames/bind";
import style from "./admin.module.scss";
import Tippy from "@tippyjs/react/headless";
import { useEffect, useRef, useState } from "react";
import { Wrapper } from "../../../components/Popper";
import PopperAdmin from "./popperAdmin/popperAdmin";

const cx = classNames.bind(style);

function Admin() {
  const popupRef = useRef();
  const wrapperRef = useRef();
  const buttonRef = useRef();

  const [showPopup, setShowPopup] = useState(false);

  const handleClosePopup = () => {
    setShowPopup(false);
  };

  const handleTogglePopup = () => {
    setShowPopup((prev) => !prev);
  };


  useEffect(() => {
    const handleClickOutside = (e) => {
      if (
        popupRef.current &&
        !popupRef.current.contains(e.target) &&
        buttonRef.current &&
        !buttonRef.current.contains(e.target)
      ) {
        setShowPopup(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  return (
    <div className={cx("wrapper")} ref={wrapperRef}>
      <Tippy
        interactive
        visible={showPopup}
        placement="bottom-end"
        offset={[50, 5]}
        render={(attrs) => (
          <div
            className={cx("popper-wrapper")}
            ref={popupRef}
            tabIndex="-1"
            {...attrs}
          >
            <PopperAdmin
              className={cx("Detail-Popup")}
              onClose={handleClosePopup}
            />
          </div>
        )}
        
      >
        <div className={cx("actions")}  onClick={() => setShowPopup((prev) => !prev)}  ref={buttonRef}  >
          <div className={cx("content")}>
            <div className={cx("admin")}>Admin</div>
            <div className={cx("item-admin")}>A</div>
          </div>
        </div>
      </Tippy>
    </div>
  );
}

export default Admin;
