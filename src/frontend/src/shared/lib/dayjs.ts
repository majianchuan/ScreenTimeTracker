import dayjs from "dayjs";

import localizedFormat from "dayjs/plugin/localizedFormat";
import isSameOrAfter from "dayjs/plugin/isSameOrAfter";
import isoWeek from "dayjs/plugin/isoWeek";

import "dayjs/locale/zh-cn";
import "dayjs/locale/en";

dayjs.extend(isSameOrAfter);
dayjs.extend(isoWeek);
dayjs.extend(localizedFormat);

export default dayjs;
