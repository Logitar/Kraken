import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCheck,
  faChessRook,
  faCircleInfo,
  faEdit,
  faGear,
  faHome,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faStar,
  faTimes,
  faUser,
  faUsersGear,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCheck,
  faChessRook,
  faCircleInfo,
  faEdit,
  faGear,
  faHome,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faStar,
  faTimes,
  faUser,
  faUsersGear,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
